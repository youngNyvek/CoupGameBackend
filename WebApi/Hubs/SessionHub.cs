using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class SessionHub : Hub
    {
        private readonly SessionService _service;

        public SessionHub(SessionService service)
        {
            _service = service;
        }

        // Método para criar uma nova sessão
        public async Task<string> CreateSession(string nickname)
        {
            var sessionCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper(); // Gera um código único

            // Cria uma nova sessão associada ao sessionCode
            var session = new SessionEntity
            {
                SessionCode = sessionCode,
                Players = new List<PlayerEntity>
                {
                  new PlayerEntity
                  {
                      ConnectionId = Context.ConnectionId,
                      Nickname = nickname
                  }
                }
            };

            await _service.SaveSessionAsync(session); // Salva a sessão no repositório

            // Adiciona o cliente ao grupo correspondente à sessão
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);

            // Retorna o sessionCode para o cliente
            return sessionCode;
        }

        // Método para entrar em uma sessão existente
        public async Task JoinSession(string sessionCode, string nickname)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Sessão não encontrada");
                return;
            }

            // Adiciona o jogador à lista de jogadores da sessão
            var player = new PlayerEntity
            {
                ConnectionId = Context.ConnectionId,
                Nickname = nickname
            };
            session.Players.Add(player);

            await _service.SaveSessionAsync(session); // Salva a sessão atualizada

            // Adiciona o cliente ao grupo correspondente à sessão
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);

            // Notifica todos os clientes sobre a lista atualizada de jogadores
            await Clients.Group(sessionCode).SendAsync("UpdatePlayers", session.Players);

            // Envia o estado atual do contador para o cliente que acabou de entrar
            await Clients.Caller.SendAsync("UpdateCounter", session.Counter);
        }

        // Incrementa o contador
        public async Task IncrementCounter(string sessionCode)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session != null)
            {
                session.Counter++;
                await _service.SaveSessionAsync(session);

                // Notifica todos os clientes no grupo sobre a atualização do contador
                await Clients.Group(sessionCode).SendAsync("UpdateCounter", session.Counter);
            }
        }

        // Decrementa o contador
        public async Task DecrementCounter(string sessionCode)
        {
            var session = await _service.GetSessionAsync(sessionCode);
            if (session != null)
            {
                session.Counter--;
                await _service.SaveSessionAsync(session);

                // Notifica todos os clientes no grupo sobre a atualização do contador
                await Clients.Group(sessionCode).SendAsync("UpdateCounter", session.Counter);
            }
        }

        // Remove o jogador quando ele se desconecta
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var session = await _service.RemovePlayerByConnectionId(Context.ConnectionId);
            if (session != null)
            {
                // Notifica todos os clientes sobre a lista atualizada de jogadores
                await Clients.Group(session.SessionCode).SendAsync("UpdatePlayers", session.Players);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}