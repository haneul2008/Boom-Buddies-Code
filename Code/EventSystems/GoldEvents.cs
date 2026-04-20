using System;

namespace Code.EventSystems
{
    public static class GoldEvents
    {
        public static readonly GoldChangeEvent GoldChangeEvent = new GoldChangeEvent();
        public static readonly GoldSetEvent GoldSetEvent = new GoldSetEvent();
        public static readonly SetAddGoldEvent SetAddGoldEvent = new SetAddGoldEvent();
    }

    public class GoldChangeEvent : GameEvent
    {
        public int gold;
        public Action<bool> onChangeGold;

        public GoldChangeEvent Initializer(int gold)
        {
            this.gold = gold;
            return this;
        }
    }
    
    public class GoldSetEvent : GameEvent
    {
        public int gold;

        public GoldSetEvent Initializer(int gold)
        {
            this.gold = gold;
            return this;
        }
    }
    
    public class SetAddGoldEvent : GameEvent
    {
        public int gold;

        public SetAddGoldEvent Initializer(int gold)
        {
            this.gold = gold;
            return this;
        }
    }
}