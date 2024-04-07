using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class Player
    {
        public readonly Entity Entity;
        public List<Card> Pile;
        public readonly List<Card> Discard = new();
        public readonly List<Card> Hand = new();
        private readonly Deck _deck;

        public Player(int health, Deck deck)
        {
            _deck = deck;
            Entity = new Entity(health);
        }

        public int Energy { get; private set; }
        public int EnergyPerTurn => 3;

        public void StartTurn()
        {
            Entity.OnStartTurn();
            Energy = EnergyPerTurn;

            // Draw 5
            for (var i = 0; i < 5; i++) DrawCard();
        }

        public void EndTurn()
        {
            // Discard hand
            Discard.AddRange(Hand);
            Hand.Clear();
        }

        public void DrawCard()
        {
            if (Pile.Count == 0)
            {
                // Shuffle discard into deck
                Pile.AddRange(Discard);
                Shuffle(Pile);
                Discard.Clear();
            }

            if (Pile.Count == 0)
                // No cards to draw
                return;

            var card = Pile[0];
            Pile.RemoveAt(0);
            Hand.Add(card);
        }

        public void UseEnergy(int energy)
        {
            Energy = Mathf.Max(0, Energy - energy);
        }

        private void Shuffle(List<Card> cards)
        {
            // Fisher-Yates shuffle
            for (var i = cards.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public void Reset()
        {
            Pile = _deck.CreatePile();
            Shuffle(Pile);
            Hand.Clear();
            Discard.Clear();
        }
    }
}