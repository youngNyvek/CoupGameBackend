using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class SessionHub(SessionService service, GameActionsService gameActions) : Hub
    {
        private readonly SessionService _service = service;
        private readonly GameActionsService _gameActions = gameActions;

        // Mantém o método de criar sessão
        public async Task<string> CreateSession()
        {
            var sessionCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            
            var session = new SessionEntity { 
                SessionCode = sessionCode
            };

            await _service.SaveSessionAsync(session);
            return sessionCode;
        }

        // Mantém o método de entrar na sessão
        public async Task JoinSession(string sessionCode, string nickname)
        {
            var session = await _service.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            var player = new PlayerEntity
            {
                ConnectionId = Context.ConnectionId,
                Nickname = nickname,
                Coins = 2,
            };

            session.Players.Add(player);
            await _service.SaveSessionAsync(session);

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);
            await Clients.Group(sessionCode).SendAsync("UpdatePlayers", session.Players);
        }

        public async Task DeclareAction(string sessionCode, string actionName, string? targetId)
        {
            var actionEntity = await _gameActions.HandleDeclareAction(sessionCode, Context.ConnectionId, actionName, targetId);

            await Clients.Group(sessionCode).SendAsync("Notify", new
            {
                type = "ActionDeclared",
                actionEntity
            });
        }

        public async Task DeclareCounterAction(string sessionCode, string counterActionName)
        {
            var actionEntity = await _gameActions.HandleDeclareCounterAction(sessionCode, Context.ConnectionId, counterActionName);

            await Clients.Group(sessionCode).SendAsync("Notify", new
            {
                type = "CounterActionDeclared",
                actionEntity
            });
        }

        public async Task DeclareChallenge(string sessionCode)
        {
            await _gameActions.HandleDeclareChallenge(sessionCode);

            await Clients.Group(sessionCode).SendAsync("Notify", new
            {
                type = "ChallengeDeclared"
            });
        }

        public async Task ExecuteAction(string sessionCode)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Sessão não encontrada");
                return;
            }

            await _gameActions.HandleExecuteAction(sessionCode);

            await Clients.Group(sessionCode).SendAsync("UpdatePlayers", session.Players);
            await Clients.Group(sessionCode).SendAsync("ActionDone");
        }

        public async Task ExecuteCounterAction(string sessionCode)
        {
        }

        public async Task ExecuteChallenge(string sessionCode)
        {
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var session = await _service.RemovePlayerByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                await Clients.Group(session.SessionCode).SendAsync("UpdatePlayers", session.Players);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
