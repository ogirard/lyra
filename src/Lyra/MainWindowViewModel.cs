using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DynamicData.Binding;
using Lyra.Features.Search;
using Lyra.Features.Songs;
using Lyra.UI;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Lyra
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly ILogger<MainWindowViewModel> logger;
        private readonly ISongRepository songRepository;
        private readonly ISearchService searchService;

        private ObservableCollection<SongViewModel> songs;
        private ObservableCollection<string> songTags;
        private SongViewModel selectedSong;
        private ObservableCollection<object> searchItems;
        private string searchText;
        private string presentSongButtonText;
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
                var view = CollectionViewSource.GetDefaultView(Songs);
                view.Filter = FilterSong;
                return view;
            }
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

        public SongPresenterViewModel SongPresenterViewModel { get; }

        public ICommand PresentSongCommand { get; }

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ISongRepository songRepository, ISearchService searchService, SongPresenterViewModel songPresenterViewModel)
        {
            this.logger = logger;
            this.songRepository = songRepository;
            this.searchService = searchService;

            Songs = new ObservableCollection<SongViewModel>();
            SongTags = new ObservableCollection<string>();
            PresentSongCommand = ReactiveCommand.Create(PresentSong, this.WhenAnyValue(x => x.SelectedSong).Select(x => x != null));
            SongPresenterViewModel = songPresenterViewModel;
            this.WhenValueChanged(x => x.SelectedSong, false).Subscribe(selectedSong =>
            {
                PresentSongButtonText = selectedSong != null ? $"Lied '{selectedSong.Number} {selectedSong.Title}' anzeigen" : "Kein Lied ausgewählt";
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

        public void Activate()
        {
            var songs = songRepository.GetSongs();
            Songs.Clear();
            foreach (var songViewModel in songs.Select(s => new SongViewModel(s)))
            {
                Songs.Add(songViewModel);
            }

            SongTags.Clear();
            foreach (var tag in songs.SelectMany(s => s.Tags).Distinct().OrderBy(x => x))
            {
                SongTags.Add(tag);
            }

            this.WhenValueChanged(x => x.SearchText, false)
                .Throttle(TimeSpan.FromSeconds(0.8), RxApp.TaskpoolScheduler)
                .Select(query => query?.Trim())
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    ExecuteSearchAndRefresh();
                });

            this.WhenValueChanged(x => x.SearchItems, false).Subscribe(_ =>
            {
                ExecuteSearchAndRefresh();
            });
        }

        private void ExecuteSearchAndRefresh()
        {
            searchResults = this.searchService
                .Search(searchText, searchItems?.Cast<string>(), songRepository.GetSongs())
                .Where(x => x.IsMatch)
                .ToDictionary(x => x.Song.Id, x => x);

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