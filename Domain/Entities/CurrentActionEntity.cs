using Domain.Enums;

namespace Domain.Entities
{
    public class CurrentActionEntity : ActionEntity
    {
        public string? TargetPlayerId { get; set; }
        public IEnumerable<CounterActionChoicesEnum> CounterActionChoices { get; set; } = [];
    }
}
