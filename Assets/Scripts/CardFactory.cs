using System.Collections.Generic;

public class CardFactory
{
    private Dictionary<string, CardBlueprint> _cardBlueprints = new();

    public CardFactory(CardBlueprint[] cardBlueprints)
    {
        foreach (var blueprint in cardBlueprints)
        {
            _cardBlueprints.Add(blueprint.Id, blueprint);
        }
    }

    public Card CreateCard(string id)
    {
        return _cardBlueprints[id].CreateCard();
    }
}