namespace Effects
{
    public interface IEffect
    {
        public void Perform(Entity author, Entity target);
        public string GetDescription(Entity author);
    }
}