using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using DynamicData.Binding;
using Lyra.UI;
using ReactiveUI;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Shared;

namespace Lyra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromelessWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new FluentTheme { ThemeName = "FluentLight", ShowAcrylicBackground = true });
            MaxWidth = int.MaxValue;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ViewModel.Activate();
            Presenter = new SongPresenter
            {
                Owner = this,
                ViewModel = ViewModel.SongPresenterViewModel,
                WindowState = WindowState.Maximized,
            };
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
    }
}
