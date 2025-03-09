namespace Domain.Entities
{
    public class GamePhaseEntity
    {
        public CurrentActionEntity? CurrentAction { get; set; }
        public ActionEntity? CounterAction { get; set; }
        public bool IsActionChallenged { get; set; }
    }
}
