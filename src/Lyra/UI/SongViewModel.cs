using System.Linq;
using Lyra.Features.Songs;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongViewModel : ReactiveObject
    {
        private readonly MainWindowViewModel root;
        private float score;

        public string Id => Song.Id;

        public float Score
        {
            get => score;
            set => this.RaiseAndSetIfChanged(ref score, value);
        }

        public int Number => Song.Number;

        public string Title => Song.Title;

        public string Text => Song.Text;

        public string ToolTip => $"{Song.Number} - {Song.Title}";

        public Song Song { get; }

        public PresentationStyleViewModel PresentationStyle { get; }

        public SongViewModel(Song song, MainWindowViewModel root)
        {
            this.Song = song;
            this.root = root;
            this.PresentationStyle = root.Styles.FirstOrDefault(s => s.Id == song.StyleId);
        }
    }
}