using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.References
{
    [Serializable]
    public struct SpawnableStatPair
    {
        public SpawnableDataSO spawnableData;
        public List<StatSO> spawnableStatList;
    }

    [CreateAssetMenu(fileName = "SpawnableStatList", menuName = "SO/SpawnableStatList", order = 0)]
    public class SpawnableStatListSO : ScriptableObject
    {
        public List<SpawnableStatPair> statPairList = new List<SpawnableStatPair>();

        public List<StatSO> GetStatList(SpawnableDataSO spawnableData)
        {
            foreach (SpawnableStatPair pair in statPairList)
            {
                if (pair.spawnableData == spawnableData)
                    return pair.spawnableStatList;
            }

            return null;
        }
    }
}