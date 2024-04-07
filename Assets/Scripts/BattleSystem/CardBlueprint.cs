using Effects;

namespace BattleSystem
{
    public class CardBlueprint
    {
        public string Name => _name;
        private readonly IEffect[] _effects;
        private readonly int _energyCost;
        private readonly string _name;
        private readonly TargetingType _targetingType;

        public CardBlueprint(string name, int energyCost, TargetingType targetingType, params IEffect[] effects)
        {
            _name = name;
            _energyCost = energyCost;
            _targetingType = targetingType;
            _effects = effects;
        }

        public string Id => _name.ToLower().Replace(" ", "_");

        public Card CreateCard()
        {
            return new Card(_name, Id, _targetingType, _energyCost, _effects);
        }
    }
}