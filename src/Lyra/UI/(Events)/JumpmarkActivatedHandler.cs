using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lyra.UI
{
    public class JumpmarkActivatedHandler : INotificationHandler<JumpmarkActivated>
    {
        private readonly ILogger<JumpmarkActivatedHandler> logger;
        private readonly SongPresenterViewModel songPresenterViewModel;

        public JumpmarkActivatedHandler(ILogger<JumpmarkActivatedHandler> logger, SongPresenterViewModel songPresenterViewModel)
        {
            this.logger = logger;
            this.songPresenterViewModel = songPresenterViewModel;
        }

        public Task Handle(JumpmarkActivated jumpmarkActivated, CancellationToken cancellationToken)
        {
            if (songPresenterViewModel.Document == null || this.songPresenterViewModel.PresentedSong?.Id != jumpmarkActivated.SongId)
            {
                return Task.CompletedTask;
            }

            var section = (Section)songPresenterViewModel.Document.Blocks.FirstBlock;
            var paragraph = section.Blocks.OfType<Paragraph>().FirstOrDefault(x => x.Name == jumpmarkActivated.Name);
            if (paragraph != null)
            {
                paragraph.BringIntoView();
            }

            return Task.CompletedTask;
        }
    }
}