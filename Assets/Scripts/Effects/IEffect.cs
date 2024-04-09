using BattleSystem;

namespace Effects
{
    public interface IEffect
    {
        public TargetingType TargetingType { get; }
        public void Perform(Entity author, Entity target);
        public string GetDescription(Entity author);
    }
}