namespace Code.Scenes.Initializer
{
    public interface ISceneInit
    {
        public int Priority { get; }
        
        public void OnSceneInit();

        public void OnSceneExit();
    }
}