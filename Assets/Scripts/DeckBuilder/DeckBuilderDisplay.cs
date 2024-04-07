using System.Collections.Generic;
using BattleSystem;
using Display;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckBuilderDisplay : MonoBehaviour
{
    [SerializeField] private CardLayout collectionView;
    [SerializeField] private CardLayout deckView;

    private void Start()
    {
        var cardFactory = GlobalState.GetCardFactory();
        collectionView.Show(cardFactory.GetSearch());
        
        var deck = GlobalState.GetDeck();
        deckView.Show(deck.CreatePile());
    }

    public void BackToMainMenu()
    {
        MainMenuStart.LaodMainMenu();
    }
}