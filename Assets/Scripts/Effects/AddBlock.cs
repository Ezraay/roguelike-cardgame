using UnityEngine;

namespace Effects
{
    public class AddBlock : IEffect
    {
        [SerializeField] private readonly int _block;

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