using Domain.Entities;
using Domain.Interfaces;
using System;
using static System.Collections.Specialized.BitVector32;

namespace Application.Services
{
    public class SessionService
    {
        private readonly ISessionRepository _repository;

        public SessionService(ISessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<SessionEntity> CreateSessionAsync()
        {
            var session = new SessionEntity();
            await _repository.SaveSessionAsync(session);
            return session;
        }

        public async Task<SessionEntity?> GetSessionAsync(string sessionCode)
        {
            return await _repository.GetSessionAsync(sessionCode);
        }
        
        // Salva ou atualiza uma sessão
        public async Task SaveSessionAsync(SessionEntity session)
        {
            await _repository.SaveSessionAsync(session);
        }

        public async Task<SessionEntity?> RemovePlayerByConnectionId(string connectionId)
        {
            var sessions = await _repository.GetAllSessionsAsync();

            var playerWithSession = sessions
                .SelectMany(s => s.Players.Select(p => new { Player = p, Session = s }))
                .FirstOrDefault(ps => ps.Player.ConnectionId == connectionId);

            if (playerWithSession == null)
            {
                Console.WriteLine($"Sessão não encontrada para ConnectionId: {connectionId}");
                return null;
            }

            var session = playerWithSession.Session;
            var player = playerWithSession.Player;

            session.Players.Remove(player);
            Console.WriteLine($"Jogador {player.Nickname} removido da sessão {session.SessionCode}");

            await _repository.SaveSessionAsync(session);

            return session;
        }
    }
}
