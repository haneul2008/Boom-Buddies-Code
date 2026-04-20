using System;
using System.Collections.Generic;
using System.Linq;
using Code.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Entities
{
    public class EntityStatCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected List<StatOverride> statOverrides;

        protected StatSO[] _stats;
        protected Entity _entity;

        public StatSO GetStat(StatSO targetStat) => _stats.FirstOrDefault(stat => stat.statName == targetStat.statName);
        
        [Serializable]
        public struct StatSaveData
        {
            public string statName;
            public float baseValue;
            public List<ModifyData> modifyDatas;
        }
        
        [Serializable]
        public struct ModifyData
        {
            public string key;
            public float value;
        }

        public void Initialize(Entity entity)
        {
            _entity = entity;
            
            _stats = statOverrides.Select(stat => stat.CreateStat()).ToArray();
        }

        [ContextMenu("Print Stat")]
        public void PrintStat()
        {
            foreach (StatSO stat in _stats)
            {
                print($"{stat.statName} : {stat.Value}");
            }
        }
    }
}