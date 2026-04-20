using Code.Core;
using Code.Entities;
using UnityEngine;

namespace Code.Units
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "SO/Unit/UnitData", order = 0)]
    public class UnitDataSO : SpawnableDataSO
    {
        public int cost;
    }
}