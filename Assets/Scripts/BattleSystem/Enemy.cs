using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class Enemy : Entity
    {
        private readonly List<Card> _intents = new();
        private readonly string[] _cards;

        public Enemy(string name, int maxHealth, string[] cards) : base(name, maxHealth)
        {
            _cards = cards;
        }

        public void CreateIntents(CardFactory cardFactory)
        {
            _intents.Clear();
            _intents.Add(cardFactory.CreateCard(_cards[Random.Range(0, _cards.Length)]));
        }

        public List<Card> GetIntents()
        {
            return _intents;
        }
    }
}