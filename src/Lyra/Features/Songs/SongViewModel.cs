using Lyra.Framework.MVVM;

namespace Lyra.Features.Songs
{
    public class SongViewModel : ViewModelBase
    {
        private readonly Song song;

        public string Number => song.Number.ToString().PadLeft(5, ' ');

        public string Title => song.Title;

        public string Text => song.Text;

        public SongViewModel(Song song)
        {
            this.song = song;
        }
    }
}