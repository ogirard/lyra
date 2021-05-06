using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Lyra.Features.Config;
using Lyra.Features.SessionTracking;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Lyra.UI
{
    public class SongPresenterViewModel : ReactiveObject
    {
        private readonly ILogger<SongPresenterViewModel> logger;

        public SongPresenterViewModel(ILogger<SongPresenterViewModel> logger, ISessionTrackingService sessionTrackingService, IPresenterConfigService presenterConfigService)
        {
            this.logger = logger;
            this.WhenAnyValue(x => x.PresentedSong)
                .Subscribe(
                x =>
                {
                    if (x != null && IsPresentationActive)
                    {
                        sessionTrackingService.LogStartPresentation(x.Song);
                        Document = new SongFlowDocument(PresentedSong, PresentedSong.PresentationStyle, false);
                        PreviousSong = Playlist?.FindSong(PresentedSong, -1);
                        NextSong = Playlist?.FindSong(PresentedSong, 1);
                    }
                    else
                    {
                        Document = null;
                        PreviousSong = null;
                        NextSong = null;
                    }
                });

            this.WhenAnyValue(x => x.IsPresentationActive).Subscribe(x =>
            {
                if (x)
                {
                    sessionTrackingService.LogStartPresentation(PresentedSong.Song);
                    Document = new SongFlowDocument(PresentedSong, PresentedSong.PresentationStyle, false);
                }
                else
                {
                    sessionTrackingService.LogEndPresentation();
                    Document = null;
                    PreviousSong = null;
                    NextSong = null;
                }
            });

            AvailableScreens = new ObservableCollection<PresenterScreen>(presenterConfigService.GetScreens());
            Screen = AvailableScreens.FirstOrDefault(x => x.ScreenId == presenterConfigService.GetSelectedPresenterScreen()?.ScreenId);
            this.WhenAnyValue(x => x.Screen).Subscribe(presenterConfigService.SelectPresenterScreen);

            PresentNextSongCommand = ReactiveCommand.Create(PresentNextSong, this.WhenAnyValue(x => x.IsPresentationActive));
            PresentPreviousSongCommand = ReactiveCommand.Create(PresentPreviousSong, this.WhenAnyValue(x => x.IsPresentationActive));
            ClosePresenterCommand = ReactiveCommand.Create(() => { IsPresentationActive = false; }, this.WhenAnyValue(x => x.IsPresentationActive));
        }

        public ICommand PresentNextSongCommand { get; }

        public ICommand PresentPreviousSongCommand { get; }

        public ICommand ClosePresenterCommand { get; set; }

        [Reactive]
        public SongPlaylistViewModel Playlist { get; set; }

        [Reactive]
        public SongViewModel PresentedSong { get; set; }

        [Reactive]
        public SongViewModel PreviousSong { get; set; }

        [Reactive]
        public SongViewModel NextSong { get; set; }

        [Reactive]
        public bool IsPresentationActive { get; set; }

        [Reactive]
        public ObservableCollection<PresenterScreen> AvailableScreens { get; set; }

        [Reactive]
        public PresenterScreen Screen { get; set; }

        [Reactive]
        public SongFlowDocument Document { get; set; }

        private void PresentPreviousSong()
        {
            if (!IsPresentationActive)
            {
                logger.LogDebug("Cannot present previous song, not presenting at the moment.");
                return;
            }

            if (PreviousSong == null)
            {
                logger.LogDebug("Cannot present previous song, no previous song defined.");
                return;
            }

            PresentedSong = PreviousSong;
        }

        private void PresentNextSong()
        {
            if (!IsPresentationActive)
            {
                logger.LogDebug("Cannot present next song, not presenting at the moment.");
                return;
            }

            if (NextSong == null)
            {
                logger.LogDebug("Cannot present next song, no next song defined.");
                return;
            }

            PresentedSong = NextSong;
        }

        public void HandleKeyboardShortcut(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    IsPresentationActive = false;
                    logger.LogDebug($"Shortcut pressed on {nameof(SongPresenter)}: [ESC]. Closing presenter.");
                    e.Handled = true;
                    break;
            }
        }
    }
}