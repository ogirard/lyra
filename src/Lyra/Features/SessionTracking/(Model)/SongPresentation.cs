using System;

namespace Lyra.Features.SessionTracking
{
    public class SongPresentation
    {
        public DateTime PresentationStarted { get; set; }

        public DateTime PresentationEnded { get; set; }

        public TimeSpan Duration => PresentationEnded - PresentationStarted;

        public string Presenter { get; set; }

        public string SongId { get; set; }

        public int SongNumber { get; set; }

        public string SongTitle { get; set; }
    }
}