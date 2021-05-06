using System;
using System.Windows;
using System.Windows.Input;
using DynamicData.Binding;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;

namespace Lyra.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromelessWindow
    {
        public MainWindow(SongPresenter songPresenter)
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new FluentTheme { ThemeName = "FluentLight", ShowAcrylicBackground = true });
            MaxWidth = int.MaxValue;
            Loaded += OnLoaded;
            Presenter = songPresenter;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize();
            Presenter.Owner = this;
            Presenter.ViewModel = ViewModel.SongPresenterViewModel;
            this.ViewModel.SongPresenterViewModel.WhenValueChanged(x => x.IsPresentationActive, false).Subscribe(isPresentationActive =>
            {
                if (isPresentationActive)
                {
                    Presenter.Show();
                }
                else
                {
                    Presenter.Hide();
                }
            });
        }

        public MainWindowViewModel ViewModel
        {
            get => DataContext as MainWindowViewModel;
            set => DataContext = value;
        }

        public SongPresenter Presenter { get; set; }

        private void OnMouseDoubleClickListView(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && ViewModel.StartPresentationFromSelectedSongCommand.CanExecute(null))
            {
                ViewModel.StartPresentationFromSelectedSongCommand.Execute(null);
            }
        }
    }
}
