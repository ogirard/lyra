using System;
using System.Windows.Input;
using Lyra.Features.SessionTracking;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongPresenterViewModel : ReactiveObject
    {
        private readonly ILogger<SongPresenterViewModel> logger;
        private SongViewModel presentedSong;
        private bool isPresentationActive;

        public SongPresenterViewModel(ILogger<SongPresenterViewModel> logger, ISessionTrackingService sessionTrackingService)
        {
            this.logger = logger;
            this.WhenAnyValue(x => x.PresentedSong)
                .Subscribe(
                x =>
                {
                    if (x != null && isPresentationActive)
                    {
                        sessionTrackingService.LogStartPresentation(x.Song);
                    }
                });

            this.WhenAnyValue(x => x.IsPresentationActive).Subscribe(x =>
            {
                if (x)
                {
                    sessionTrackingService.LogStartPresentation(PresentedSong.Song);
                }
                else
                {
                    sessionTrackingService.LogEndPresentation();
                }
            });
        }

        public SongViewModel PresentedSong
        {
            get => presentedSong;
            set => this.RaiseAndSetIfChanged(ref presentedSong, value);
        }

        public bool IsPresentationActive
        {
            get => isPresentationActive;
            set => this.RaiseAndSetIfChanged(ref isPresentationActive, value);
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