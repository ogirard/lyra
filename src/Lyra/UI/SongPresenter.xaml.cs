using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Lyra.Features.Config;
using Microsoft.Extensions.Logging;
using ReactiveUI;

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
            this.WhenAnyValue(x => x.ViewModel.Screen).Subscribe(x =>
            {
                WindowState = WindowState.Normal;
                this.Top = x.Bounds.Top;
                this.Left = x.Bounds.Left;
                this.Width = x.Bounds.Width;
                this.Height = x.Bounds.Height;
                WindowState = WindowState.Maximized;
                logger.LogTrace($"Presenting on screen '{x.DeviceName}', bounds: {x.Bounds}");
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.IsPresentationActive = false;
            e.Cancel = true;
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
