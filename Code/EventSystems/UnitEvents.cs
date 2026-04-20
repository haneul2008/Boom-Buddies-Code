using Code.Units;

namespace Code.EventSystems
{
    public static class UnitEvents
    {
        public static readonly LastUnitSpawnEvent LastUnitSpawnEvent = new LastUnitSpawnEvent();
        public static readonly AllUnitDeadEvent AllUnitDeadEvent = new AllUnitDeadEvent();
        public static readonly UnitSpawnEvent UnitSpawnEvent = new UnitSpawnEvent();
        public static readonly CheckLastUnitSpawnEvent CheckLastUnitSpawnEvent = new CheckLastUnitSpawnEvent();
    }
    
    public class CheckLastUnitSpawnEvent : GameEvent
    {}
    
    public class AllUnitDeadEvent : GameEvent
    {}

    public class UnitSpawnEvent : GameEvent
    {
        public UnitDataSO unitData;

        public UnitSpawnEvent Initializer(UnitDataSO unitData)
        {
            this.unitData = unitData;
            return this;
        }
    }

    public class LastUnitSpawnEvent : GameEvent
    {
    }
}