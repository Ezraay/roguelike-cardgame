using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class Enemy : Entity
    {
        private readonly List<Card> _intents = new();

        public Enemy(int maxHealth) : base(maxHealth)
        {
        }

        public void CreateIntents(CardFactory cardFactory)
        {
            _intents.Clear();
            _intents.Add(Random.value < 0.5f
                ? cardFactory.CreateCard("strike")
                : cardFactory.CreateCard("defend"));
        }

        public List<Card> GetIntents()
        {
            return _intents;
        }
    }
}