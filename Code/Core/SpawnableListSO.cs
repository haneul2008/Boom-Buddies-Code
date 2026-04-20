using System.Collections.Generic;
using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(fileName = "SpawnableList", menuName = "SO/Spawnable/SpawnableList", order = 0)]
    public class SpawnableListSO : ScriptableObject
    {
        public List<SpawnableDataSO> spawnableDataList;
    }
}