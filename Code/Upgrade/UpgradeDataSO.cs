using System;
using System.Collections.Generic;
using Code.Stats;
using UnityEngine;

namespace Code.Upgrade
{
    [Serializable]
    public struct UpgradeStat
    {
        public StatSO targetStat;
        public float modifyValue;
    }
    
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "SO/Upgrade/UpgradeData", order = 0)]
    public class UpgradeDataSO : ScriptableObject
    {
        public List<UpgradeStat> upgrades;
        public Mesh targetMesh;
        public int requiredGold;
        public Sprite upgradeSprite;
    }
}