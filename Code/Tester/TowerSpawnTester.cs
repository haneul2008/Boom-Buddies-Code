using Code.Managers;
using Code.Towers;
using UnityEngine;

namespace Code.Tester
{
    public class TowerSpawnTester : MonoBehaviour
    {
        [SerializeField] private TowerDataSO towerData;

        [ContextMenu("Spawn Tower")]
        public void SpawnTower()
        {
            TowerManager towerManager = CreateOnceManager.Instance.GetManager<TowerManager>();
            towerManager.Create(towerData);
        }
    }
}