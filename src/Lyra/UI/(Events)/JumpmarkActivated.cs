using MediatR;

namespace Lyra.UI
{
    public class JumpmarkActivated : INotification
    {
        public string Name { get; set; }

        public string SongId { get; set; }
    }
}