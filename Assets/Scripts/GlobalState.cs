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
    private static LevelFactory _levelFactory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        // Read file from StreamingAssets into string
        var cardsYaml = Resources.Load<TextAsset>(CardsPath).text;
        _cardFactory = new CardFactory(cardsYaml);
        
        var levelsYaml = Resources.Load<TextAsset>("Data/levels").text;
        _levelFactory = new LevelFactory(levelsYaml);
        
        _playerDeck = DeckSave.LoadDeck(_cardFactory);
    }

    public static Deck GetDeck()
    {
        return _playerDeck;
    }
    
    public static LevelFactory GetLevelFactory()
    {
        return _levelFactory;
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