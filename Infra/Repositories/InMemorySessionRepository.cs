using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Interfaces;

namespace Infra.Repositories
{
    public class InMemorySessionRepository : ISessionRepository
    {
        private readonly ConcurrentDictionary<string, SessionEntity> _sessions = new();

        public Task<SessionEntity?> GetSessionAsync(string sessionCode)
        {
            _sessions.TryGetValue(sessionCode, out var session);
            return Task.FromResult(session);
        }

        public Task SaveSessionAsync(SessionEntity session)
        {
            _sessions[session.SessionCode] = session; // Salva ou atualiza a sessão
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(string sessionCode)
        {
            _sessions.TryRemove(sessionCode, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<SessionEntity>> GetAllSessionsAsync()
        {
            return Task.FromResult(_sessions.Values.AsEnumerable());
        }
    }
}
