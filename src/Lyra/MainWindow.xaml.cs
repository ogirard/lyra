using System;
using Lyra.Framework.MVVM;

namespace Lyra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LyraWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
