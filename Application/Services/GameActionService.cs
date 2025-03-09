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
                CounterActionChoices = action.CounterActionChoices
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
 
        public async Task<(bool nextTurn, bool finishGame)> HandleFinishTurn(string sessionCode)
        {
            var session = await _sessionService.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            List<PlayerEntity> players = session.Players;

            var defeatedPlayers = players.Where(p => p.Cards.All(c => c.Exposed));

            if (defeatedPlayers.Count() == players.Count - 1)
                return (nextTurn: false, finishGame: true);

            var gamephase = session.GamePhase;

            var currentPlayerIndex = players.FindIndex(s => s.ConnectionId == gamephase.CurrentAction.ActorPlayerId);

            var nextPlayer = currentPlayerIndex + 1 % players.Count;

            gamephase = new()
            {
                CurrentAction = new() 
                { 
                    ActorPlayerId = session.Players[nextPlayer].ConnectionId,
                },
                CounterAction = null,
                IsActionChallenged = false,
            };

            return (nextTurn: true, finishGame: false);
        }
    }
}
