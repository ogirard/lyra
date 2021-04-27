using System.Linq;
using Lyra.Features.Songs;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongViewModel : ReactiveObject
    {
        private readonly Song song;
        private readonly MainWindowViewModel root;
        private decimal rank;

        public string Id => song.Id;

        public decimal Rank
        {
            get => rank;
            set => this.RaiseAndSetIfChanged(ref rank, value);
        }

        public int Number => song.Number;

        public string Title => song.Title;

        public string Text => song.Text;

        public string ToolTip => $"{song.Number} - {song.Title}";

        public PresentationStyleViewModel PresentationStyle { get; }

        public SongViewModel(Song song, MainWindowViewModel root)
        {
            this.song = song;
            this.root = root;
            this.PresentationStyle = root.Styles.FirstOrDefault(s => s.Id == song.StyleId);
        }
    }
}