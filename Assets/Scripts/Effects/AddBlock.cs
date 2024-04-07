using BattleSystem;

namespace Effects
{
    public class AddBlock : IEffect
    {
        private readonly int _block;

        public AddBlock(int block)
        {
            _block = block;
        }

        public void Perform(Entity author, Entity target)
        {
            target.AddBlock(_block);
        }

        public string GetDescription(Entity author)
        {
            return $"Gain {_block} block.";
        }
    }
}