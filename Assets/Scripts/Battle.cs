using System.Collections.Generic;
using Effects;
using UnityEngine;

public class Battle
{
    private readonly CardFactory _cardFactory;
    public readonly List<Enemy> Enemies = new();
    public readonly Player Player;

    public Battle(CardFactory cardFactory, Player player)
    {
        _cardFactory = cardFactory;
        Player = player;

        SpawnEnemies();

        foreach (var enemy in Enemies)
            enemy.OnDeath += () =>
            {
                Enemies.Remove(enemy);

                if (Enemies.Count == 0)
                    // TODO End battle
                    Debug.Log("Battle is won!");
            };
        CreateIntents();
    }

    public void Start()
    {
        Player.StartTurn();
    }

    public bool UseCard(Card card, Entity target)
    {
        if (Player.Energy < card.EnergyCost) return false;
        Player.UseEnergy(card.EnergyCost);
        PerformCard(card, Player.Entity, target);
        return true;
    }

    public void EndTurn()
    {
        Player.EndTurn();
        foreach (var enemy in Enemies)
            enemy.OnStartTurn();

        PerformEnemyTurns();
        CreateIntents();
        Player.StartTurn();
    }

    private void PerformEnemyTurns()
    {
        foreach (var enemy in Enemies)
        {
            var intent = enemy.GetIntents();
            foreach (var card in intent) PerformCard(card, enemy, Player.Entity);
        }

        CreateIntents();
    }

    private void PerformCard(Card card, Entity author, Entity target)
    {
        switch (card.TargetingType)
        {
            case TargetingType.Self:
                target = author;
                break;
        }

        card.Use(author, target);
    }

    private void SpawnEnemies()
    {
        // TODO Move to dedicated behaviour class
        Enemies.Add(new Enemy(20));
        Enemies.Add(new Enemy(15));
    }

    private void CreateIntents()
    {
        // TODO Move to dedicated behaviour class
        foreach (var enemy in Enemies) enemy.CreateIntents(_cardFactory);
    }
}