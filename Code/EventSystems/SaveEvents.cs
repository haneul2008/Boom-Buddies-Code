namespace Code.EventSystems
{
    public static class SaveEvents
    {
        public static readonly SaveEvent SaveEvent = new SaveEvent();
        public static readonly LoadEvent LoadEvent = new LoadEvent();
        public static readonly SaveToDatabaseEvent SaveToDatabaseEvent = new SaveToDatabaseEvent();
    }

    public class SaveEvent : GameEvent
    {
        public bool isSaveToFile;

        public SaveEvent Initializer(bool isSaveToFile)
        {
            this.isSaveToFile = isSaveToFile;
            return this;
        }
    }

    public class LoadEvent : GameEvent
    {
        public bool isLoadFromFile;

        public LoadEvent Initializer(bool isLoadFromFile)
        {
            this.isLoadFromFile = isLoadFromFile;
            return this;
        }
    }
    
    public class SaveToDatabaseEvent : GameEvent
    {
    }
}