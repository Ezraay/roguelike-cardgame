using Effects;

namespace BattleSystem
{
    public class CardBlueprint
    {
        public string Name => _name;
        private readonly IEffect[] _effects;
        private readonly int _cost;
        private readonly string _name;
        private readonly TargetingType _targetingType;
        public string Id { get; }

        public CardBlueprint(string id, string name, int cost, TargetingType targetingType, params IEffect[] effects)
        {
            Id = id;
            _name = name;
            _cost = cost;
            _targetingType = targetingType;
            _effects = effects;
        }

        public Card CreateCard()
        {
            return new Card(_name, Id, _targetingType, _cost, _effects);
        }
    }
}