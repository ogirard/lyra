using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.SessionTracking
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ILogger<SessionRepository> logger;
        private readonly ILiteRepository dbRepository;
        private readonly string sessionCollectionName;

        public SessionRepository(ILogger<SessionRepository> logger, ILiteRepository dbRepository)
        {
            this.logger = logger;
            this.dbRepository = dbRepository;
            sessionCollectionName = this.dbRepository.Database.GetCollection<Session>().Name;
        }

        public Session GetSession(string id)
            => dbRepository.SingleOrDefault<Session>(s => s.Id == id);

        public IReadOnlyCollection<Session> GetSessions()
            => dbRepository.Fetch<Session>(_ => true).ToList();

        public void AddSession(Session session)
        {
            dbRepository.Insert(session);
            logger.LogTrace($"Added {nameof(Session)} '{session.Id}' to '{sessionCollectionName}' collection");
        }

        public void RemoveSession(Session session)
            => dbRepository.Delete<Session>(session.Id);
    }
}