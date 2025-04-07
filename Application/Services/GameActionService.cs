using Application.Factories;
using Domain.Entities;

namespace Application.Services
{
    public class GameActionsService(SessionService sessionService)
    {
        private readonly SessionService _sessionService = sessionService;

        public async Task<CurrentActionEntity> HandleDeclareAction(string sessionCode, string actorPlayerId, string actionName, string? targetId)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");
           
            var gamePhase = session.GamePhase;

            var action = GameActionFactory.CreateAction(actionName);

            var currentAction = new CurrentActionEntity()
            {
                ActorPlayerId = actorPlayerId,
                ActionName = actionName,
                TargetPlayerId = targetId,
                CounterActionChoices = action.CounterActionChoices,
                CanBeChallenged = action.CanBeChallenged
            };

            gamePhase.CurrentAction = currentAction;

            return currentAction;
        }

        public async Task<ActionEntity> HandleDeclareCounterAction(string sessionCode, string actorPlayerId, string counterActionName)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            var counterAction = new ActionEntity()
            {
                ActionName = counterActionName,
                ActorPlayerId = actorPlayerId
            };

            session.GamePhase.CounterAction = counterAction;

            await _sessionService.SaveSessionAsync(session);

            return counterAction;
        }

        public async Task HandleDeclareChallenge(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            session.GamePhase.IsActionChallenged = true;
                
            await _sessionService.SaveSessionAsync(session);
        }

        public async Task HandleExecuteAction(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            var gamephase = session.GamePhase;
            var currentAction = gamephase.CurrentAction;

            var action = GameActionFactory.CreateAction(currentAction.ActionName);

            action.Execute(session, currentAction);

            await _sessionService.SaveSessionAsync(session);
        }
    }
}