namespace Domain.Entities
{
    public record CardEntity()
    {
        public required string Name { get; set; }
        public bool Exposed { get; set; } = false;
    }
}
