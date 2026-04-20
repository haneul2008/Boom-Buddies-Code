using Code.Entities;
using Code.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Towers
{
    public struct TowerInfo
    {
        public float? cooldown;
        public float? damage;
        public float? range;
        public int? health;
    }

    public class TowerStatCompo : EntityStatCompo
    {
        [SerializeField] private StatSO cooldownStat;
        [SerializeField] private StatSO damageStat;
        [SerializeField] private StatSO healthStat;
        [SerializeField] private StatSO rangeStat;

        public TowerInfo GetInfo()
        {
            return new TowerInfo()
            {
                cooldown = GetStat(cooldownStat)?.Value,
                damage = GetStat(damageStat)?.Value,
                range = GetStat(rangeStat)?.Value,
                health = GetStat(healthStat) != null? (int)GetStat(healthStat).Value : null,
            };
        }
    }
}