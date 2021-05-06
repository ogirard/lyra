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
using Lyra.Features.Styles;
using Lyra.Framework;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Lyra.UI
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly ILogger<MainWindowViewModel> logger;
        private readonly IMediator mediator;
        private readonly ISongRepository songRepository;
        private readonly ISearchService searchService;
        private readonly IStyleRepository styleRepository;
        private Dictionary<string, SearchResult> searchResults = new();

        [Reactive]
        public ObservableCollection<SongViewModel> Songs { get; set; }

        public ListCollectionView SongsView { get; }

        [Reactive]
        public string SongListInfo { get; set; }

        [Reactive]
        public ObservableCollection<PresentationStyleViewModel> Styles { get; set; }

        [Reactive]
        public SongViewModel SelectedSong { get; set; }

        [Reactive]
        public ObservableCollection<string> AvailableSongTags { get; set; }

        [Reactive]
        public ObservableCollection<object> SelectedSongTags { get; set; }

        [Reactive]
        public string SearchText { get; set; }

        [Reactive]
        public string PresentSongButtonText { get; set; }

        [Reactive]
        public string WindowTitle { get; set; }

        public SongPresenterViewModel SongPresenterViewModel { get; }

        public ICommand StartPresentationFromSelectedSongCommand { get; }

        public ICommand ClearSearchCommand { get; }

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IOptionsMonitor<LyraOptions> options,
            IMediator mediator,
            ISongRepository songRepository,
            ISearchService searchService,
            IStyleRepository styleRepository,
            SongPresenterViewModel songPresenterViewModel)
        {
            this.logger = logger;
            this.songRepository = songRepository;
            this.searchService = searchService;
            this.styleRepository = styleRepository;
            this.mediator = mediator;

            Songs = new ObservableCollection<SongViewModel>();
            SongsView = new ListCollectionView(Songs) { Filter = FilterSong };
            SongsView.SortDescriptions.Add(new SortDescription(nameof(SongViewModel.Score), ListSortDirection.Descending));
            SongsView.SortDescriptions.Add(new SortDescription(nameof(SongViewModel.Number), ListSortDirection.Ascending));
            AvailableSongTags = new ObservableCollection<string>();
            Styles = new ObservableCollection<PresentationStyleViewModel>();
            StartPresentationFromSelectedSongCommand = ReactiveCommand.Create(StartPresentationFromSelectedSong, this.WhenAnyValue(x => x.SelectedSong).Select(x => x != null));
            ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
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

        public void Initialize()
        {
            Styles.Clear();
            foreach (var style in styleRepository.GetStyles().Select(s => new PresentationStyleViewModel(s)))
            {
                Styles.Add(style);
            }

            var songs = songRepository.GetSongs();
            Songs.Clear();

            foreach (var songViewModel in songs.Select(s => new SongViewModel(s, this, mediator)))
            {
                Songs.Add(songViewModel);
            }

            AvailableSongTags.Clear();
            foreach (var tag in songs.SelectMany(s => s.Tags).Distinct().Concat(new[] { "Test", "Another" }).OrderBy(x => x))
            {
                AvailableSongTags.Add(tag);
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

            this.WhenAnyValue(x => x.SelectedSongTags)
                .Subscribe(x =>
                {
                    ExecuteSearchAndRefresh();
                    x?.ObserveCollectionChanges().Subscribe(_ =>
                    {
                        ExecuteSearchAndRefresh();
                    });
                });
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedSongTags?.Clear();
        }

        private void StartPresentationFromSelectedSong()
        {
            if (SelectedSong == null)
            {
                logger.LogDebug("Cannot present, no song selected.");
                SongPresenterViewModel.IsPresentationActive = false;
                return;
            }

            logger.LogDebug($"Start playlist '{SongListInfo}' and present song '{SelectedSong.Song.DisplayText}'");
            SongPresenterViewModel.Playlist = SongPlaylistViewModel.FromSongsView(SongListInfo, SongsView);
            SongPresenterViewModel.PresentedSong = SelectedSong;
            SongPresenterViewModel.IsPresentationActive = true;
        }

        private void ExecuteSearchAndRefresh()
        {
            searchResults = this.searchService
                .Search(SearchText, SelectedSongTags?.Cast<string>().ToList(), songRepository.GetSongs())
                .Where(x => x.IsMatch)
                .ToDictionary(x => x.Song.Id, x => x);

            Songs.ForEach(s => s.Score = searchResults.ContainsKey(s.Id) ? searchResults[s.Id].Score : 0f);

            SongListInfo = (!SelectedSongTags?.Any() ?? true) && string.IsNullOrEmpty(SearchText)
                ? $"Alle {Songs.Count} Lieder"
                : $"{searchResults.Count} von {Songs.Count} Lieder";
            SongsView.Refresh();
        }

        private bool FilterSong(object item)
        {
            if ((!SelectedSongTags?.Any() ?? true) && string.IsNullOrEmpty(SearchText))
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