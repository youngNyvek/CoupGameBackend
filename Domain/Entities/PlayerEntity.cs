namespace Domain.Entities
{
    public class PlayerEntity
    {
        public string ConnectionId { get; set; } = string.Empty; // ID da conexão do SignalR
        public string Nickname { get; set; } = string.Empty;     // Nome do jogador
        public int Coins { get; set; }
        public IEnumerable<Card> Cards { get; set; } = [];
        
        public bool HasCard(Card card)
        {
            return Cards.Contains(card);
        }
    }

    public record Card
    {
        public required string Name { get; set; }
        public bool Exposed { get; set; }
    }
}