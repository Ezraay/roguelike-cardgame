using System.Collections.Generic;
using Effects;
using UnityEngine;

public class Battle
{
    public readonly Dictionary<Entity, List<Card>> Intents = new();
    public readonly List<Entity> Enemies = new();
    public readonly Player Player;

    public Battle()
    {
        Player = new Player(100, new List<Card>
        {
            new("Strike", TargetingType.Enemy, 1, new IEffect[] { new DealDamage(6) }),
            new("Strike", TargetingType.Enemy, 1, new IEffect[] { new DealDamage(6) }),
            new("Strike", TargetingType.Enemy, 1, new IEffect[] { new DealDamage(6) }),
            new("Strike", TargetingType.Enemy, 1, new IEffect[] { new DealDamage(6) }),
            new("Strike", TargetingType.Enemy, 1, new IEffect[] { new DealDamage(6) }),
            new("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) }),
            new("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) }),
            new("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) }),
            new("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) }),
            new("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) })
        });
        Enemies.Add(new Entity(20));
        Enemies.Add(new Entity(15));

        foreach (var enemy in Enemies) Intents.Add(enemy, new List<Card>());
        CreateIntents();
    }

    public void Start()
    {
        Player.StartTurn();
    }

    public void UseCard(Card card, Entity target)
    {
        Player.UseEnergy(1);
        PerformCard(card, Player.Entity, target);
    }

    public void EndTurn()
    {
        Player.EndTurn();
        PerformEnemyTurns();
        CreateIntents();
        Player.StartTurn();
    }

    private void PerformEnemyTurns()
    {
        foreach (var enemy in Enemies)
        {
            var intent = Intents[enemy];
            foreach (var card in intent) PerformCard(card, enemy, Player.Entity);
        }

        CreateIntents();
    }

    private void PerformCard(Card card, Entity author, Entity target)
    {
        switch (card.TargetingType)
        {
            case TargetingType.RandomAlly:
            case TargetingType.Self:
                target = Player.Entity;
                break;
        }

        card.Use(author, target);
    }

    private void CreateIntents()
    {
        // TODO Move to dedicated behaviour class
        foreach (var intent in Intents.Values)
        {
            intent.Clear();
            intent.Add(Random.value < 0.5f
                ? new Card("Strike", TargetingType.RandomAlly, 1, new IEffect[] { new DealDamage(6) })
                : new Card("Defend", TargetingType.Self, 1, new IEffect[] { new AddBlock(5) }));
        }
    }
}