using System;
using System.Collections.Generic;
using Effects;

namespace BattleSystem
{
    public class Card
    {
        private readonly Guid _guid;
        private readonly IEffect[] _effects;
        public readonly int EnergyCost;
        public readonly string Name;
        public readonly string Id;
        public readonly TargetingType TargetingType;


        public Card(string name, string id, TargetingType targetingType, int energyCost, IEffect[] effects)
        {
            _guid = Guid.NewGuid();
            Name = name;
            Id = id;
            TargetingType = targetingType;
            EnergyCost = energyCost;
            _effects = effects;
        }

        public void Use(Entity author, Entity target, IEnumerable<Entity> enemies)
        {
            foreach (var effect in _effects)
                switch (effect.TargetingType)
                {
                    case TargetingType.Enemy:
                    {
                        effect.Perform(author, target);
                        break;
                    }
                    case TargetingType.Self:
                    {
                        effect.Perform(author, author);
                        break;
                    }
                    case TargetingType.AllEnemies:
                    {
                        foreach (var enemy in enemies)
                            effect.Perform(author, enemy);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        public string GetDescription()
        {
            var description = "";
            foreach (var effect in _effects)
            {
                var line = effect.GetDescription(null);
                description += line;
            }

            return description;
            // return string.Join(Environment.NewLine, Array.ConvertAll(_effects, effect => effect.GetDescription(null, TODO)));
        }

        public Card Copy()
        {
            var effects = new IEffect[_effects.Length];
            _effects.CopyTo(effects, 0);
            return new Card(Name, Id, TargetingType, EnergyCost, effects);
        }

        public override bool Equals(object obj)
        {
            if (obj is Card card) return _guid == card._guid;

            return false;
        }
    }
}