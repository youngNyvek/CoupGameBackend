using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISessionRepository
    {
        Task<SessionEntity?> GetSessionAsync(string sessionCode);
        Task SaveSessionAsync(SessionEntity session);
        Task DeleteSessionAsync(string sessionCode);
        Task<IEnumerable<SessionEntity>> GetAllSessionsAsync();
    }
}
