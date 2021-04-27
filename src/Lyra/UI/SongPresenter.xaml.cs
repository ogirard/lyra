using System.Windows;
using System.Windows.Input;
using Lyra.Features.Config;
using Microsoft.Extensions.Logging;

namespace Lyra.UI
{
    /// <summary>
    /// Interaction logic for SongPresenter.xaml
    /// </summary>
    public partial class SongPresenter : Window
    {
        private readonly ILogger<SongPresenter> logger;
        private readonly IPresenterConfigService presenterConfigService;

        public SongPresenter(ILogger<SongPresenter> logger, IPresenterConfigService presenterConfigService)
        {
            this.logger = logger;
            this.presenterConfigService = presenterConfigService;
            InitializeComponent();
            Loaded += OnLoaded;
            ShowInTaskbar = false;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var screen = presenterConfigService.GetSelectedPresenterScreen();
            this.Top = screen.Bounds.Top;
            this.Left = screen.Bounds.Left;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;
            WindowState = WindowState.Maximized;
            logger.LogTrace($"Presenting on screen '{screen.DeviceName}', bounds: {screen.Bounds}");
        }

        public SongPresenterViewModel ViewModel
        {
            get => DataContext as SongPresenterViewModel;
            set => DataContext = value;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.HandleKeyboardShortcut(e);
        }
    }
}
