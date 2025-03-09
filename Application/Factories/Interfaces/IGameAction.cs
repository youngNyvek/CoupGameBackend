using Domain.Entities;
using Domain.Enums;

namespace Application.Factories.Interfaces
{
    public interface IGameAction
    {
        void Execute(SessionEntity session, ActionEntity actionEntity);
        bool CanBeChallenged { get; } // Define se a ação pode ser contestada
        IEnumerable<CounterActionChoicesEnum> CounterActionChoices { get => []; }// Define se a ação pode ser contestada
    }
}