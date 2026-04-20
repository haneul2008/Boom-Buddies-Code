namespace Code.Managers
{
    public interface IOnceManager
    {
        public int Priority { get; }
        public void Initialize();
    }
}