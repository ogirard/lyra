using System.Windows.Input;
using Lyra.Features.Songs;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongPresenterViewModel : ReactiveObject
    {
        private readonly ILogger<SongPresenterViewModel> logger;
        private SongViewModel presentedSong;
        private bool isPresentationActive;

        public SongPresenterViewModel(ILogger<SongPresenterViewModel> logger)
        {
            this.logger = logger;
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