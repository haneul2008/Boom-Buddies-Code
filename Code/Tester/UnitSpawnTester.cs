using Code.Managers;
using Code.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Tester
{
    public class UnitSpawnTester : MonoBehaviour
    {
        [SerializeField] private UnitDataSO unitData;

        [ContextMenu("Spawn Unit")]
        public void SpawnUnit()
        {
            UnitManager unitManager = CreateOnceManager.Instance.GetManager<UnitManager>();
            unitManager.Create(unitData);
        }
    }
}