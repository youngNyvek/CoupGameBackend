using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;

namespace WebApi.Hubs
{
    public class SessionHub(SessionService service, GameActionsService gameActions, GameManagerService gameManager) : Hub
    {
        private readonly SessionService _service = service;
        private readonly GameActionsService _gameActions = gameActions;
        private readonly GameManagerService _gameManager = gameManager;

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

        public async Task StartGame(string sessionCode)
        {
            var session = await _gameManager.StartGame(sessionCode);

            foreach (var player in session.Players)
            {
                await Clients.Client(player.ConnectionId).SendAsync("StartGame", player.Deck);
            }   
        }

        // Mantém o método de entrar na sessão
        public async Task JoinSession(string sessionCode, string nickname)
        {
            var session = await _service.GetSessionAsync(sessionCode) ?? throw new Exception("Sessão não encontrada");

            var player = new PlayerEntity
            {
                ConnectionId = Context.ConnectionId,
                Nickname = nickname
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

        public async Task HandleFinishTurn(string sessionCode)
        {
            var session = await _service.GetSessionAsync(sessionCode);

            await _gameManager.HandleFinishTurn(sessionCode);
            
            await Clients.Group(sessionCode).SendAsync("UpdatePlayers", session.Players);
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
