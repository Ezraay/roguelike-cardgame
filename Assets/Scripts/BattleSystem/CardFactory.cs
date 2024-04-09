using System;
using System.Collections.Generic;
using System.Linq;
using Effects;
using YamlDotNet.Serialization;

namespace BattleSystem
{
    public class CardFactory
    {
        private readonly Dictionary<string, CardBlueprint> _cardBlueprints = new();

        public CardFactory(string cardData)
        {
            var deserializer = new DeserializerBuilder()
                .Build();
            var cardDatas = deserializer.Deserialize<CardData[]>(cardData);
            var cards = cardDatas.Select(x => x.CreateCardBlueprint());
            foreach (var blueprint in cards) _cardBlueprints.Add(blueprint.Id, blueprint);
        }

        public Card CreateCard(string id)
        {
            return _cardBlueprints[id].CreateCard();
        }

        public List<Card> GetSearch(string query = "")
        {
            return _cardBlueprints.Values.Where(cardBlueprint => cardBlueprint.Name.Contains(query))
                .Select(cardBlueprint => cardBlueprint.CreateCard()).ToList();
        }
        
        private struct CardData
        {
            public string name;
            public string id;
            public int cost;
            public string[] types;
            public EffectData[] effects;

            public CardBlueprint CreateCardBlueprint()
            {
                var effects = this.effects.Select(x => x.CreateEffect()).ToArray();
                var targetingType = effects.Max(x => x.TargetingType);
                return new CardBlueprint(id, name, cost, targetingType, effects);
            }
        }

        private struct EffectData
        {
            public string target;
            public string effect;
            public int amount;

            public IEffect CreateEffect()
            {
                var target = this.target switch
                {
                    "enemy" => TargetingType.Enemy,
                    "self" => TargetingType.Self,
                    "all-enemies" => TargetingType.AllEnemies,
                    _ => throw new ArgumentOutOfRangeException()
                };

                IEffect effect = this.effect switch
                {
                    "damage" => new DealDamage(amount, target),
                    "block" => new AddBlock(amount, target),
                    _ => throw new ArgumentOutOfRangeException()
                };

                return effect;
            }
        }
    }
}