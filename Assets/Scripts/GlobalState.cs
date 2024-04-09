using System;
using System.Linq;
using BattleSystem;
using Effects;
using UnityEngine;
using YamlDotNet.Serialization;

public static class GlobalState
{
    private const string CardsPath = "Data/cards";
    private static Deck _playerDeck;
    private static CardFactory _cardFactory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        // Read file from StreamingAssets into string
        var cardsYaml = Resources.Load<TextAsset>(CardsPath).text;
        

        _cardFactory = new CardFactory(cardsYaml);
    }

    public static Deck GetDeck()
    {
        _playerDeck ??= DeckSave.LoadDeck(_cardFactory);
        return _playerDeck;
    }

    public static void SaveDeck(Deck deck)
    {
        _playerDeck = deck;
        DeckSave.SaveDeck(deck);
    }

    public static CardFactory GetCardFactory()
    {
        return _cardFactory;
    }

    
}