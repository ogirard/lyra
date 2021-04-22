using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Lyra.Features.Songs;
using Lyra.Framework.MVVM;
using Microsoft.Extensions.Logging;

namespace Lyra
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ILogger<MainWindowViewModel> logger;
        private readonly ISongRepository songRepository;

        private ObservableCollection<SongViewModel> songs;
        private SongViewModel selectedSong;

        public ICommand LoadSongsCommand { get; set; }

        public ObservableCollection<SongViewModel> Songs
        {
            get => songs;
            set => SetPropertyValue(value, ref songs);
        }

        public SongViewModel SelectedSong
        {
            get => selectedSong;
            set => SetPropertyValue(value, ref selectedSong);
        }

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ISongRepository songRepository)
        {
            this.logger = logger;
            this.songRepository = songRepository;

            Songs = new ObservableCollection<SongViewModel>();
        }

        protected override void OnInitialize()
        {
            foreach (var songViewModel in songRepository.GetSongs().Select(s => new SongViewModel(s)))
            {
                Songs.Add(songViewModel);
            }
        }
    }
}