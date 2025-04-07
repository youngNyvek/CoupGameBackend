using Application.Factories.Interfaces;

namespace Application.Factories
{
    public static class GameActionFactory
    {
        private static readonly Dictionary<string, Func<IGameAction>> _actionMap = new()
        {
            { "TAX", () => new Actions.DukeTaxAction() },
        };

        public static IGameAction CreateAction(string actionName)
        {
            if (_actionMap.TryGetValue(actionName, out var action))
            {
                return action();
            }

            throw new InvalidOperationException($"Ação '{actionName}' não encontrada.");
        }
    }
}
