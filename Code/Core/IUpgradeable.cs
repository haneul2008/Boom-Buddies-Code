using Code.Upgrade;

namespace Code.Core
{
    public interface IUpgradeable
    {
        public UpgradeDataListSO MyUpgrade { get; }
        public int UpgradeLevel { get; set; }

        public void Upgrade(UpgradeDataSO upgradeData);
    }
}