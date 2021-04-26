using ReactiveUI;

namespace Lyra.Features.Songs
{
    public class SongViewModel : ReactiveObject
    {
        private readonly Song song;

        public string Id => song.Id;

        public string Number => song.Number.ToString();

        public string Title => song.Title;

        public string Text => song.Text;

        public SongViewModel(Song song)
        {
            this.song = song;
        }
    }
}