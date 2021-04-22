using System;
using System.Collections.Generic;
using System.Linq;

namespace Lyra.Features.SessionTracking
{
    public class Session
    {
        public string Id { get; set; }

        public DateTime SessionStarted => Presentations.Min(p => p.PresentationStarted);

        public DateTime SessionEnded => Presentations.Min(p => p.PresentationEnded);

        public TimeSpan SessionDuration => SessionEnded - SessionStarted;

        public List<SongPresentation> Presentations { get; set; } = new();
    }
}