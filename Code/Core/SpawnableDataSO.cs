using Code.Upgrade;
using UnityEngine;

namespace Code.Core
{
    public abstract class SpawnableDataSO : ScriptableObject
    {
        public UpgradeDataListSO upgrades;
        public GameObject prefab;
        public Sprite spawnableSprite;
        public string spawnableName;
        public int requiredGold;
        [TextArea] public string desc;
    }
}