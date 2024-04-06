using System.Collections.Generic;

public class Deck
{
    private List<Card> _cards;
    
    public Deck(List<Card> cards)
    {
        this._cards = cards;
    }

    public List<Card> CreatePile()
    {
        return new List<Card>(_cards);
    }
}