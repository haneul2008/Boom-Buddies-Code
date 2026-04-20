using System;
using Code.Entities;
using Code.Stats;
using UnityEngine;

namespace Code.Units.Impl
{
    public class Dragon : Unit
    {
        [SerializeField] private StatSO damageStat;
        [SerializeField] private float damageIncreaseValue;
        
        private bool _isDamageUpgrade;

        public override void Initialize()
        {
            base.Initialize();
            
            _health.OnHit.AddListener(HandleHit);
        }

        private void OnDestroy()
        {
            _health.OnHit.RemoveListener(HandleHit);
        }

        private void HandleHit(float current)
        {
            if (current / _health.MaxHealth < 0.5f && _isDamageUpgrade == false)
            {
                _isDamageUpgrade = true;

                EntityStatCompo statCompo = GetCompo<EntityStatCompo>(true);

                StatSO targetStat = statCompo.GetStat(damageStat);
                targetStat.AddModifier(this, damageIncreaseValue);
            }
        }
    }
}