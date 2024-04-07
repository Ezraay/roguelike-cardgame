using System;
using UnityEngine;

namespace BattleSystem
{
    public class Entity
    {
        public Entity(int maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public int Block { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public event Action OnDeath;

        public void TakeDamage(int damage)
        {
            var blockDamage = Mathf.Min(damage, Block);
            Health -= Mathf.Max(0, damage - blockDamage);
            Block -= blockDamage;

            if (Health <= 0) OnDeath?.Invoke();
        }

        public void AddBlock(int block)
        {
            Block += block;
        }

        public void OnStartTurn()
        {
            Block = 0;
        }
    }
}