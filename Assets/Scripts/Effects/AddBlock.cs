namespace Effects
{
    public class AddBlock : IEffect
    {
        public AddBlock(int block)
        {
            _block = block;
        }

        private readonly int _block;

        public void Perform(Entity author, Entity target)
        {
            target.AddBlock(_block);
        }
    }
}