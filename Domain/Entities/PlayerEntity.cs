namespace Domain.Entities
{
    public class PlayerEntity
    {
        public string ConnectionId { get; set; } = string.Empty; // ID da conexão do SignalR
        public string Nickname { get; set; } = string.Empty;     // Nome do jogador
    }
}