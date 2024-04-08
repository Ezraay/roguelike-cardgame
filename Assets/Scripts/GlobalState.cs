﻿using BattleSystem;
using Effects;

public static class GlobalState
{
    private static Deck _playerDeck;

    private static readonly CardFactory CardFactory = new(
        new CardBlueprint("Strike", 1, TargetingType.Enemy, new DealDamage(6)),
        new CardBlueprint("Defend", 1, TargetingType.Self, new AddBlock(5)),
        new CardBlueprint("Cleave", 2, TargetingType.AllEnemies, new DealDamage(4))
    );

    public static Deck GetDeck()
    {
        _playerDeck ??= DeckSave.LoadDeck(CardFactory);
        return _playerDeck;
    }

    public static void SaveDeck(Deck deck)
    {
        _playerDeck = deck;
        DeckSave.SaveDeck(deck);
    }

    public static CardFactory GetCardFactory()
    {
        return CardFactory;
    }
}