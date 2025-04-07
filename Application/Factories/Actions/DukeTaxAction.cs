using Application.Factories.Interfaces;
using Domain.Entities;

namespace Application.Factories.Actions
{
    public class DukeTaxAction : IGameAction
    {
        public bool CanBeChallenged => true;

        public void Execute(SessionEntity session, ActionEntity actionEntity)
        {
            var player = session.Players.First(p => p.ConnectionId == actionEntity.ActorPlayerId);
            player.Coins += 3;
        }
    }
}
