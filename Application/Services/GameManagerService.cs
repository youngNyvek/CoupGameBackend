using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class GameManagerService(SessionService sessionService)
    {
        private readonly SessionService _sessionService = sessionService;

        public async Task<SessionEntity> StartGame(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            session
                .DealCoins()
                .DealCards()
                .DealFirstTurn();
            
            await _sessionService.SaveSessionAsync(session);

            return session;
        }

        public async Task<(bool nextTurn, bool finishGame)> HandleFinishTurn(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            var players = session.Players.ToList();

            var defeatedPlayers = players.Where(p => p.Deck.All(c => c.Exposed));

            if (defeatedPlayers.Count() == players.Count - 1)
                return (nextTurn: false, finishGame: true);

            var gamephase = session.GamePhase;

            var currentPlayerIndex = players.FindIndex(s => s.ConnectionId == gamephase.CurrentAction.ActorPlayerId);

            var nextPlayer = currentPlayerIndex + 1 % players.Count;

            gamephase = new()
            {
                CurrentAction = new()
                {
                    ActorPlayerId = players[nextPlayer].ConnectionId,
                },
                CounterAction = null,
                IsActionChallenged = false,
            };

            await _sessionService.SaveSessionAsync(session);

            return (nextTurn: true, finishGame: false);
        }
    }
}
