using System.Collections.Generic;
using UnityEngine;

namespace Code.Upgrade
{
    [CreateAssetMenu(fileName = "UpgradeDataList", menuName = "SO/Upgrade/List", order = 0)]
    public class UpgradeDataListSO : ScriptableObject
    {
        public List<UpgradeDataSO> upgradeList;

        public bool IsMax(int upgradeLevel) => upgradeLevel >= upgradeList.Count;
        
        public UpgradeDataSO GetUpgrade(int upgradeLevel)
        {
            if (upgradeLevel >= upgradeList.Count) return null;

            return upgradeList[upgradeLevel];
        }
    }
}