using LiteDB;
using Lyra.Features.Songs;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.SessionTracking
{
    public class SessionTrackingService : ISessionTrackingService
    {
        private readonly ILogger<SessionTrackingService> logger;
        private readonly ILiteRepository dbRepository;

        private const string StartPresentationKey = "START_PRESENTATION";
        private const string EndPresentationKey = "END_PRESENTATION";

        public SessionTrackingService(ILogger<SessionTrackingService> logger, ILiteRepository dbRepository)
        {
            this.logger = logger;
            this.dbRepository = dbRepository;
        }

        public void LogStartPresentation(Song song)
            => LogMetric(StartPresentationKey, song);

        public void LogEndPresentation()
            => LogMetric(EndPresentationKey);

        private void LogMetric(string metricName, Song song = null)
        {
            var metric = new SessionTrackingMetric { Metric = metricName };
            if (song != null)
            {
                metric.Tags.Add("Song", song);
            }

            dbRepository.Insert(metric, collectionName: metricName);
            logger.LogTrace($"Logged session metric: {metricName}{(song == null ? string.Empty : $" for song '{song.DisplayText}'")}'");
        }
    }
}