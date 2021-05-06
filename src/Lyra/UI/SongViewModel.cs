using System.Linq;
using Lyra.Features.Songs;
using MediatR;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongViewModel : ReactiveObject
    {
        private readonly MainWindowViewModel root;
        private readonly IMediator mediator;
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

        public void ActivateJumpmark(string name)
            => mediator.Publish(new JumpmarkActivated { Name = name, SongId = Song.Id });

        public SongViewModel(Song song, MainWindowViewModel root, IMediator mediator)
        {
            this.Song = song;
            this.root = root;
            this.mediator = mediator;
            this.PresentationStyle = root.Styles.FirstOrDefault(s => s.Id == song.StyleId);
        }
    }
}