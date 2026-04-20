using System;
using UnityEngine;

namespace Code.Stats
{
    [Serializable]
    public class StatOverride
    {
        [SerializeField] private StatSO stat;
        [SerializeField] private bool isUseOverrideValue;
        [SerializeField] private float overrideValue;
        [SerializeField] private bool isUseOverrideMinMax;
        [SerializeField] private float overrideMinValue, overrideMaxValue;
        
        public StatOverride(StatSO stat) => this.stat = stat;

        public StatSO CreateStat()
        {
            StatSO newStat = stat.Clone() as StatSO;
            Debug.Assert(newStat != null, $"{stat.statName} clone failed");

            if (isUseOverrideMinMax)
            {
                newStat.minValue = overrideMinValue;
                newStat.maxValue = overrideMaxValue;
            }

            if (isUseOverrideValue)
                newStat.BaseValue = overrideValue;
            
            return newStat;
        }

        public StatSO GetOverrideStat() => stat;
        public float GetBaseValue() => overrideValue;
    }
}