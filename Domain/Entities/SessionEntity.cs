namespace Domain.Entities
{
    public class SessionEntity
    {
        public string SessionCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(); // Código único da sessão
        public List<PlayerEntity> Players { get; set; } = new(); // Lista de jogadores conectados à sessão
        public GamePhaseEntity? GamePhase { get; set; }
    }
}