using System.Collections.Generic;

namespace BattleSystem
{
    public class Deck
    {
        private readonly Card[] _cards;

        public Deck(params Card[] cards)
        {
            _cards = cards;
        }

        public List<Card> CreatePile()
        {
            return new List<Card>(_cards);
        }
    }
}