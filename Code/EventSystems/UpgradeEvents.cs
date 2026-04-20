using System;
using Code.Core;

namespace Code.EventSystems
{
    public static class UpgradeEvents
    {
        public static readonly TryUpgradeEvent TryUpgradeEvent = new TryUpgradeEvent();
    }

    public class TryUpgradeEvent : GameEvent
    {
        public Action<bool> onSuccess;
        public IUpgradeable upgradeable;

        public TryUpgradeEvent Initializer(IUpgradeable upgradeable)
        {
            this.upgradeable = upgradeable;
            return this;
        }
    }
}