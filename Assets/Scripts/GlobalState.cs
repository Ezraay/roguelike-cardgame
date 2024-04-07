using BattleSystem;
using Effects;

public static class GlobalState
{
    private static Deck _playerDeck;

    private static readonly CardFactory CardFactory = new(
        new CardBlueprint("Strike", 1, TargetingType.Enemy, new DealDamage(6)),
        new CardBlueprint("Defend", 1, TargetingType.Self, new AddBlock(5))
    );

    public static Deck GetDeck()
    {
        _playerDeck ??= new Deck(
            CardFactory.CreateCard("strike"),
            CardFactory.CreateCard("strike"),
            CardFactory.CreateCard("strike"),
            CardFactory.CreateCard("strike"),
            CardFactory.CreateCard("strike"),
            CardFactory.CreateCard("defend"),
            CardFactory.CreateCard("defend"),
            CardFactory.CreateCard("defend"),
            CardFactory.CreateCard("defend"),
            CardFactory.CreateCard("defend")
        );
        return _playerDeck;
    }

    public static void SaveDeck(Deck deck)
    {
        _playerDeck = deck;
    }

    public static CardFactory GetCardFactory()
    {
        return CardFactory;
    }
}