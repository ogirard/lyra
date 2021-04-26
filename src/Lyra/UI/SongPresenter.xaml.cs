using System.Windows;
using System.Windows.Input;

namespace Lyra.UI
{
    /// <summary>
    /// Interaction logic for SongPresenter.xaml
    /// </summary>
    public partial class SongPresenter : Window
    {
        public SongPresenter()
        {
            InitializeComponent();
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
