using System.Collections.Generic;

namespace BattleSystem
{
    public class CardFactory
    {
        private readonly Dictionary<string, CardBlueprint> _cardBlueprints = new();

        public CardFactory(params CardBlueprint[] cardBlueprints)
        {
            foreach (var blueprint in cardBlueprints) _cardBlueprints.Add(blueprint.Id, blueprint);
        }

        public Card CreateCard(string id)
        {
            return _cardBlueprints[id].CreateCard();
        }
    }
}