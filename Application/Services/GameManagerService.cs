using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class GameManagerService(SessionService sessionService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly int _maxCardsPerPlayer = 2;

        public async Task<SessionEntity> StartGame(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            DealCards(session);

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

            return (nextTurn: true, finishGame: false);
        }

        private void DealCards(SessionEntity session)
        {
            Dictionary<string, int> cardTypes = new()
            {
                { RolesEnum.Duke.ToString(), 3},
                { RolesEnum.Assassin.ToString(), 3},
                { RolesEnum.Captain.ToString(), 3},
                { RolesEnum.Ambassador.ToString(), 3},
                { RolesEnum.Countess.ToString(), 3},
            };

            foreach (var player in session.Players)
            {
                var playerDeck = new List<CardEntity>();

                for (var i = 0; i < _maxCardsPerPlayer; i++)
                {
                    var availabeCardTypes = cardTypes
                        .Where(dc => dc.Value > 0)
                        .Select(c => c.Key);

                    var rdm = new Random();

                    var sortedIndex = rdm.Next(availabeCardTypes.Count() - 1);
                    var targetKey = availabeCardTypes.ElementAt(sortedIndex);

                    playerDeck.Add(new CardEntity { Name = targetKey });

                    cardTypes[targetKey] -= 1;
                }

                player.Deck = playerDeck;
            }

            session.CentralDeck = cardTypes
                        .Where(dc => dc.Value > 0)
                        .Select(c => new CardEntity { Name = c.Key });
        }
    }
}
