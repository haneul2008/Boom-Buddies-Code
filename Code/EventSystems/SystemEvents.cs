using Code.Core;

namespace Code.EventSystems
{
    public static class SystemEvents
    {
        public static readonly PlaceStartEvent PlaceStartEvent = new PlaceStartEvent();
        public static readonly PlaceCompleteEvent PlaceCompleteEvent = new PlaceCompleteEvent();
        public static readonly BakeMapEvent BakeMapEvent = new BakeMapEvent();
        public static readonly SpawnableBuyEvent SpawnableBuyEvent = new SpawnableBuyEvent();
        public static readonly SpawnableSellEvent SpawnableSellEvent = new SpawnableSellEvent();
    }

    public class PlaceStartEvent : GameEvent
    {
        public IPlaceable placeable;

        public PlaceStartEvent Initializer(IPlaceable placeable)
        {
            this.placeable = placeable;
            return this;
        }
    }
    
    public class PlaceCompleteEvent : GameEvent
    {
        public IPlaceable placeable;

        public PlaceCompleteEvent Initializer(IPlaceable placeable)
        {
            this.placeable = placeable;
            return this;
        }
    }

    public class BakeMapEvent : GameEvent
    {
    }
    
    public class SpawnableBuyEvent : GameEvent
    {
        public SpawnableDataSO spawnableData;

        public SpawnableBuyEvent Initializer(SpawnableDataSO spawnableData)
        {
            this.spawnableData = spawnableData;
            return this;
        }
    }

    public class SpawnableSellEvent : GameEvent
    {
        public ISpawnable spawnable;
        public IUpgradeable upgradeable;

        public SpawnableSellEvent Initializer(ISpawnable spawnable, IUpgradeable upgradeable)
        {
            this.spawnable = spawnable;
            this.upgradeable = upgradeable;
            return this;
        }
    }
}