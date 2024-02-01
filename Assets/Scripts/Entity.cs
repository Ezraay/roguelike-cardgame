using UnityEngine;

public class Entity
{
    public int Block { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    
    public Entity(int maxHealth)
    {
        MaxHealth = maxHealth;
        Health = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        var blockDamage = Mathf.Min(damage, Block);
        Health -= Mathf.Max(0, damage - blockDamage);
        Block -= blockDamage;
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