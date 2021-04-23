using System;
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

        public MainWindowViewModel ViewModel
        {
            get => DataContext as MainWindowViewModel;
            set => DataContext = value;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ViewModel?.Initialize();
        }
    }
}
