namespace Application.Services.Ports
{
    public record DeclareActionOutput
    {
        public string ActionName { get; init; }
        public string ActorId { get; init; }
        public string TargetId { get; init; }
        public IEnumerable<string> CounterActionChoices { get; init; }
    }
}
