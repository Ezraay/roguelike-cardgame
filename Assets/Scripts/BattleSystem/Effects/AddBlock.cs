using BattleSystem;

namespace Effects
{
    public class AddBlock : IEffect
    {
        private readonly int _block;

        public AddBlock(int block, TargetingType targetingType)
        {
            _block = block;
            TargetingType = targetingType;
        }

        public TargetingType TargetingType { get; }

        public void Perform(Entity author, Entity target)
        {
            target.AddBlock(_block);
        }

        public string GetDescription(Entity author)
        {
            switch (TargetingType)
            {
                case TargetingType.Self:
                default:
                    return $"Gain {_block} block. ";
            }
        }
    }
}