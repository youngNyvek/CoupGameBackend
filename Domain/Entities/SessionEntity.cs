using Domain.Enums;

namespace Domain.Entities
{
    public class SessionEntity
    {
        private readonly int _maxCardsPerPlayer = 2;

        public string SessionCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(); // Código único da sessão
        public List<PlayerEntity> Players { get; set; } = []; // Lista de jogadores conectados à sessão
        public GamePhaseEntity? GamePhase { get; set; }
        public IEnumerable<CardEntity> CentralDeck { get; set; } = [];

        public SessionEntity DealCoins()
        {
            foreach (var player in Players)
            {
                player.Coins = 2;
            }

            return this;
        }

        public SessionEntity DealCards()
        {
            Dictionary<string, int> cardTypes = new()
            {
                { RolesEnum.Duke.ToString(), 3},
                { RolesEnum.Assassin.ToString(), 3},
                { RolesEnum.Captain.ToString(), 3},
                { RolesEnum.Ambassador.ToString(), 3},
                { RolesEnum.Countess.ToString(), 3},
            };

            foreach (var player in Players)
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

            CentralDeck = cardTypes
                        .Where(dc => dc.Value > 0)
                        .Select(c => new CardEntity { Name = c.Key });

            return this;
        }

        public SessionEntity DealFirstTurn()
        {
            var rdm = new Random();

            var firstPlayerToPlay = rdm.Next(Players.Count - 1);

            GamePhase = new()
            {
                CurrentAction = new()
                {
                    ActorPlayerId = Players[firstPlayerToPlay].ConnectionId
                }
            };

            return this;
        }
    }
}