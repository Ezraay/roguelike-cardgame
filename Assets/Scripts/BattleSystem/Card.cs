﻿using System;
using Effects;

namespace BattleSystem
{
    public class Card
    {
        private readonly IEffect[] _effects;
        public readonly int EnergyCost;
        public readonly string Name;
        public readonly string Id;
        public readonly TargetingType TargetingType;


        public Card(string name, string id, TargetingType targetingType, int energyCost, IEffect[] effects)
        {
            Name = name;
            Id = id;
            TargetingType = targetingType;
            EnergyCost = energyCost;
            _effects = effects;
        }

        public void Use(Entity author, Entity target)
        {
            switch (TargetingType)
            {
                case TargetingType.Enemy:
                case TargetingType.Self:
                case TargetingType.RandomAlly:
                {
                    foreach (var effect in _effects) effect.Perform(author, target);

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
                var line = effect.GetDescription(null, TargetingType);
                description += line;
            }
            return description;
            // return string.Join(Environment.NewLine, Array.ConvertAll(_effects, effect => effect.GetDescription(null, TODO)));
        }
    }
}