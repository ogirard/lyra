using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Lyra.Features.Search;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Lyra.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Lyra
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly ILogger<MainWindowViewModel> logger;
        private readonly ISongRepository songRepository;
        private readonly ISearchService searchService;
        private readonly IStyleRepository styleRepository;

        private readonly ICollectionView songsView;
        private ObservableCollection<SongViewModel> songs;
        private ObservableCollection<string> songTags;
        private ObservableCollection<PresentationStyleViewModel> styles;
        private SongViewModel selectedSong;
        private ObservableCollection<object> searchItems;
        private string searchText;
        private string presentSongButtonText;
        private string songListInfo;
        private string windowTitle;
        private Dictionary<string, SearchResult> searchResults = new();

        public ObservableCollection<SongViewModel> Songs
        {
            get => songs;
            set => this.RaiseAndSetIfChanged(ref songs, value);
        }

        public ICollectionView SongsView
        {
            get
            {
                SongListInfo = searchResults.Count == songs.Count
                    ? $"Alle {songs.Count} Lieder"
                    : $"{searchResults.Count} von {songs.Count} Lieder";
                return songsView;
            }
        }

        public string SongListInfo
        {
            get => songListInfo;
            set => this.RaiseAndSetIfChanged(ref songListInfo, value);
        }

        public ObservableCollection<PresentationStyleViewModel> Styles
        {
            get => styles;
            set => this.RaiseAndSetIfChanged(ref styles, value);
        }

        public SongViewModel SelectedSong
        {
            get => selectedSong;
            set => this.RaiseAndSetIfChanged(ref selectedSong, value);
        }

        public ObservableCollection<string> SongTags
        {
            get => songTags;
            set => this.RaiseAndSetIfChanged(ref songTags, value);
        }

        public ObservableCollection<object> SearchItems
        {
            get => searchItems;
            set => this.RaiseAndSetIfChanged(ref searchItems, value);
        }

        public string SearchText
        {
            get => searchText;
            set => this.RaiseAndSetIfChanged(ref searchText, value);
        }

        public string PresentSongButtonText
        {
            get => presentSongButtonText;
            set => this.RaiseAndSetIfChanged(ref presentSongButtonText, value);
        }

        public string WindowTitle
        {
            get => windowTitle;
            set => this.RaiseAndSetIfChanged(ref windowTitle, value);
        }

        public SongPresenterViewModel SongPresenterViewModel { get; }

        public ICommand PresentSongCommand { get; }

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IOptionsMonitor<LyraOptions> options,
            ISongRepository songRepository,
            ISearchService searchService,
            IStyleRepository styleRepository,
            SongPresenterViewModel songPresenterViewModel)
        {
            this.logger = logger;
            this.songRepository = songRepository;
            this.searchService = searchService;
            this.styleRepository = styleRepository;

            Songs = new ObservableCollection<SongViewModel>();
            songsView = CollectionViewSource.GetDefaultView(Songs);
            songsView.Filter = FilterSong;
            songsView.SortDescriptions.Add(new SortDescription(nameof(SongViewModel.Score), ListSortDirection.Descending));
            songsView.SortDescriptions.Add(new SortDescription(nameof(SongViewModel.Number), ListSortDirection.Ascending));
            SongTags = new ObservableCollection<string>();
            Styles = new ObservableCollection<PresentationStyleViewModel>();
            PresentSongCommand = ReactiveCommand.Create(PresentSong, this.WhenAnyValue(x => x.SelectedSong).Select(x => x != null));
            SongPresenterViewModel = songPresenterViewModel;
            WindowTitle = $"Version {options.CurrentValue.Version}";
            this.WhenAnyValue(x => x.SelectedSong)
                .Subscribe(selectedSong =>
                {
                    PresentSongButtonText = selectedSong != null
                        ? $"Lied '{selectedSong.Number} {selectedSong.Title}' anzeigen"
                        : "Kein Lied ausgewählt";
                });
        }

        private void PresentSong()
        {
            if (SelectedSong == null)
            {
                logger.LogDebug($"Cannot present, no song selected.");
                SongPresenterViewModel.IsPresentationActive = false;
                return;
            }

            logger.LogDebug($"Present song '{SelectedSong.Id}'");
            SongPresenterViewModel.PresentedSong = SelectedSong;
            SongPresenterViewModel.IsPresentationActive = true;
        }

        public void Initialize()
        {
            Styles.Clear();
            foreach (var style in styleRepository.GetStyles().Select(s => new PresentationStyleViewModel(s)))
            {
                Styles.Add(style);
            }

            var songs = songRepository.GetSongs();
            Songs.Clear();
            //// LogTagsInUse(songs);

            foreach (var songViewModel in songs.Select(s => new SongViewModel(s, this)))
            {
                Songs.Add(songViewModel);
            }

            SongTags.Clear();
            foreach (var tag in songs.SelectMany(s => s.Tags).Distinct().OrderBy(x => x))
            {
                SongTags.Add(tag);
            }

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromSeconds(0.8), RxApp.TaskpoolScheduler)
                .Select(query => query?.Trim())
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    ExecuteSearchAndRefresh();
                });

            this.WhenAnyValue(x => x.SearchItems)
                .Subscribe(_ =>
                {
                    ExecuteSearchAndRefresh();
                });
        }

        private void LogTagsInUse(IReadOnlyCollection<Song> songs)
        {
            var tags = new Dictionary<string, List<string>>();
            foreach (var song in songs)
            {
                var tagOpen = false;
                var tag = string.Empty;
                foreach (var c in song.Text)
                {
                    if (c == '<')
                    {
                        tagOpen = true;
                    }
                    else if (c == '>')
                    {
                        tagOpen = false;
                        tag = tag.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).First();
                        if (!tag.StartsWith('/'))
                        {
                            if (!tags.ContainsKey(tag))
                            {
                                tags.Add(tag, new List<string>());
                            }

                            tags[tag].Add(song.Id);
                        }

                        tag = string.Empty;
                    }
                    else if (tagOpen)
                    {
                        tag += c;
                    }
                }
            }

            logger.LogInformation($"Tags in use: {string.Join($"{Environment.NewLine}  - ", tags.Select(t => $"{t.Key}: {t.Value.Count} usages, songs {string.Join(',', t.Value.Select(x => x))}"))}");
        }

        private void ExecuteSearchAndRefresh()
        {
            searchResults = this.searchService
                .Search(searchText, searchItems?.Cast<string>().ToList(), songRepository.GetSongs())
                .Where(x => x.IsMatch)
                .ToDictionary(x => x.Song.Id, x => x);

            Songs.ForEach(s => s.Score = searchResults.ContainsKey(s.Id) ? searchResults[s.Id].Score : 0f);

            SongsView.Refresh();
        }

        private bool FilterSong(object item)
        {
            if ((!searchItems?.Any() ?? true) && string.IsNullOrEmpty(searchText))
            {
                return true;
            }

            if (!searchResults.Any())
            {
                return false;
            }

            if (item is SongViewModel songViewModel)
            {
                return searchResults.ContainsKey(songViewModel.Id);
            }

            return false;
        }
    }
}