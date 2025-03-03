using Domain.Entities;

namespace Application.Services
{
    public class GameActionsService
    {
        private readonly SessionService _sessionService;

        public GameActionsService(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task IncrementCounter(SessionEntity session)
        {
            session.Counter++;
            await _sessionService.SaveSessionAsync(session);
            // Você pode notificar o Hub ou retornar algo para que o Hub notifique
        }

        public async Task DecrementCounter(SessionEntity session)
        {
            session.Counter--;
            await _sessionService.SaveSessionAsync(session);
        }

        // Exemplo de outra ação
        public async Task AttackPlayer(SessionEntity session, string targetPlayerId)
        {
            // Lógica de ataque, ex:
            // var target = session.Players.FirstOrDefault(p => p.ConnectionId == targetPlayerId);
            // target.HP -= 10; // se tiver HP
            // await _sessionService.SaveSessionAsync(session);
        }

        // Outras ações...
    }
}
