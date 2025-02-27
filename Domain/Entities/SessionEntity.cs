using System.Numerics;

namespace Domain.Entities
{
    public class SessionEntity
    {
        public string SessionCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(); // Código único da sessão
        public int Counter { get; set; } = 0; // Contador compartilhado na sessão
        public List<PlayerEntity> Players { get; set; } = new(); // Lista de jogadores conectados à sessão
    }
}