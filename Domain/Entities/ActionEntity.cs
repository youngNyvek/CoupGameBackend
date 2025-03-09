namespace Domain.Entities
{
    public class ActionEntity
    {
        public string? ActionName { get; set; }
        public required string ActorPlayerId { get; set; }
    }
}
