using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class SessionHub : Hub
    {
        private readonly SessionService _service;
        private readonly GameActionsService _gameActions;

        public SessionHub(SessionService service, GameActionsService gameActions)
        {
            _service = service;
            _gameActions = gameActions;
        }

        // Mantém o método de criar sessão
        public async Task<string> CreateSession()
        {
            var sessionCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var session = new SessionEntity { SessionCode = sessionCode };
            await _service.SaveSessionAsync(session);
            return sessionCode;
        }

        // Mantém o método de entrar na sessão
        public async Task JoinSession(string sessionCode, string nickname)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Sessão não encontrada");
                return;
            }

            var player = new PlayerEntity
            {
                ConnectionId = Context.ConnectionId,
                Nickname = nickname
            };

            session.Players.Add(player);
            await _service.SaveSessionAsync(session);

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);
            await Clients.Group(sessionCode).SendAsync("UpdatePlayers", session.Players);

            // Envia o estado atual do contador para quem acabou de entrar
            await Clients.Caller.SendAsync("UpdateCounter", session.Counter);
        }

        // Novo método genérico para executar ações
        public async Task ExecuteAction(string sessionCode, string actionName, object? payload)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Sessão não encontrada");
                return;
            }

            // Roteia para a lógica correspondente
            switch (actionName)
            {
                case "IncrementCounter":
                    await _gameActions.IncrementCounter(session);
                    // Notifica todos do grupo: ex., UpdateCounter
                    await Clients.Group(sessionCode).SendAsync("UpdateCounter", session.Counter);
                    break;

                case "DecrementCounter":
                    await _gameActions.DecrementCounter(session);
                    await Clients.Group(sessionCode).SendAsync("UpdateCounter", session.Counter);
                    break;

                //case "AttackPlayer":
                //    // Aqui assumimos que o payload traz o ID do jogador-alvo
                //    // Exemplo: { \"targetPlayerId\": \"xyz\" }
                //    if (payload is Newtonsoft.Json.Linq.JObject jObj)
                //    {
                //        var targetPlayerId = jObj["targetPlayerId"]?.ToString();
                //        if (!string.IsNullOrEmpty(targetPlayerId))
                //        {
                //            await _gameActions.AttackPlayer(session, targetPlayerId);
                //            // Notifica mudanças...
                //            await Clients.Group(sessionCode).SendAsync("SomeAttackEvent", targetPlayerId);
                //        }
                //    }
                //    break;

                default:
                    await Clients.Caller.SendAsync("Error", $"Ação não reconhecida: {actionName}");
                    break;
            }
        }

        // Mantemos OnDisconnectedAsync para remover jogadores
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
