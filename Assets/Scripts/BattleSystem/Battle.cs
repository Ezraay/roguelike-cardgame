using System;
using System.Collections.Generic;
using Effects;

namespace BattleSystem
{
    public class Battle
    {
        private readonly CardFactory _cardFactory;
        public readonly List<Enemy> Enemies = new();
        public readonly Player Player;
        public event Action OnStartEncounter;
        public event Action OnEndEncounter;

        public Battle(CardFactory cardFactory, Player player)
        {
            _cardFactory = cardFactory;
            Player = player;

        }

        public void StartEncounter(Encounter encounter)
        {
            SpawnEnemies(encounter);

            foreach (var enemy in Enemies)
                enemy.OnDeath += () =>
                {
                    Enemies.Remove(enemy);

                    if (Enemies.Count == 0)
                    {
                        OnEndEncounter?.Invoke();
                    }
                };
            CreateIntents();

            Player.Reset();
            Player.StartTurn();
            OnStartEncounter?.Invoke();
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
            IEnumerable<Entity> enemies = null;
            switch (card.TargetingType)
            {
                case TargetingType.Self:
                    target = author;
                    break;
                case TargetingType.AllEnemies:
                    enemies = author == Player.Entity ? Enemies.ConvertAll(e => (Entity) e) : new List<Entity> {Player.Entity};
                    break;
            }

            card.Use(author, target, enemies);
        }

        private void SpawnEnemies(Encounter encounter)
        {
            Enemies.AddRange(encounter.GetEnemies());
        }

        private void CreateIntents()
        {
            // TODO Move to dedicated behaviour class
            foreach (var enemy in Enemies) enemy.CreateIntents(_cardFactory);
        }
    }
}