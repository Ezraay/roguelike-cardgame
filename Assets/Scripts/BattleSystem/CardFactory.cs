using System.Collections.Generic;
using System.Linq;

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

        public List<Card> GetSearch(string query = "")
        {
            return _cardBlueprints.Values.Where(cardBlueprint => cardBlueprint.Name.Contains(query))
                .Select(cardBlueprint => cardBlueprint.CreateCard()).ToList();
        }
    }
}