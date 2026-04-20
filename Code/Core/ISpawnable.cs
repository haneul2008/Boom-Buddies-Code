namespace Code.Core
{
    public interface ISpawnable
    {
        public SpawnableDataSO SpawnableData { get; }
        public void Spawn();
    }
}