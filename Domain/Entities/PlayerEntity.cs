namespace Domain.Entities
{
    public class PlayerEntity
    {
        public string ConnectionId { get; set; } = string.Empty; // ID da conexão do SignalR
        public string Nickname { get; set; } = string.Empty;     // Nome do jogador
        public int Coins { get; set; }
        public IEnumerable<CardEntity> Deck { get; set; } = [];
        
        public bool HasCard(CardEntity card) =>
            Deck.Contains(card);
    }
}