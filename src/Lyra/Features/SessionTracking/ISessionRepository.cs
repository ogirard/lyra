using System.Collections.Generic;

namespace Lyra.Features.SessionTracking
{
    public interface ISessionRepository
    {
        Session GetSession(string id);

        IReadOnlyCollection<Session> GetSessions();

        void AddSession(Session session);

        void RemoveSession(Session session);
    }
}