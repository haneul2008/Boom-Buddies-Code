using System.Collections.Generic;
using Code.Towers.Impl;
using UnityEngine;

namespace Code.Upgrade
{
    [CreateAssetMenu(fileName = "MissileUpgradeData", menuName = "SO/Upgrade/MissileUpgradeData", order = 0)]
    public class MissileUpgradeDataSO : UpgradeDataSO
    {
        public GameObject visualPrefab;
    }
}