using System.Linq;
using Code.Entities;
using Code.Stats;
using UnityEngine;

namespace Code.Units
{
    public class UnitStatCompo : EntityStatCompo
    {
        public StatOverride GetOverrideStat(StatSO targetStat)
        {
            foreach (StatOverride statOverride in statOverrides)
            {
                StatSO overrideStat = statOverride.GetOverrideStat();
                if (overrideStat.statName == targetStat.statName) return statOverride;
            }

            return null;
        }
    }
}