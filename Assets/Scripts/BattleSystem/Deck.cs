using System.Collections.Generic;

namespace BattleSystem
{
    public class Deck
    {
        private readonly List<Card> _cards;

        public Deck(params Card[] cards)
        {
            _cards = new List<Card>(cards);
        }

        public List<Card> CreatePile()
        {
            return new List<Card>(_cards);
        }

        public bool RemoveCard(Card card)
        {
            if (_cards.Count <= 10) return false;
            for (var i = 0; i < _cards.Count; i++)
                if (_cards[i].Equals(card))
                {
                    _cards.RemoveAt(i);
                    return true;
                }

            return false;
        }

        public void AddCard(Card card)
        {
            _cards.Add(card);
        }
    }
}