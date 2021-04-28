using Lyra.Features.Songs;

namespace Lyra.Features.SessionTracking
{
    public interface ISessionTrackingService
    {
        void LogStartPresentation(Song song);

        void LogEndPresentation();
    }
}