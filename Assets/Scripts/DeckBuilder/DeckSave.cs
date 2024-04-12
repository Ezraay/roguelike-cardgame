using System.Collections.Generic;
using BattleSystem;

public class DeckSave
{
    private const string savePath = "deck";
    private static readonly List<string> defaultDeck = new() {
        "strike", "strike", "strike", "strike", "strike",
        "defend", "defend", "defend", "defend", "defend"};

    public static void SaveDeck(Deck deck)
    {
        var cards = deck.CreatePile().ConvertAll(card => card.Id);
        ES3.Save(savePath, cards);
    }

    public static Deck LoadDeck(CardFactory cardFactory)
    {
        var cards = ES3.Load(savePath, defaultDeck);
        var deck = new Deck();
        cards.ForEach(cardId =>
        {
            var card = cardFactory.CreateCard(cardId);
            if (card != null)
                deck.AddCard(card);
        });
        return deck;
    }
}