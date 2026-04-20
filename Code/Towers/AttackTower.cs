using System;
using Code.Entities;
using Code.Stats;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Towers
{
    public abstract class AttackTower : Tower
    {
        public UnityEvent OnAttackEvent;
        
        [SerializeField] protected StatSO cooldownStat;
        [SerializeField] protected StatSO damageStat;
        [SerializeField] protected Transform fireTrm;
        
        public bool CanAttack { get; set; } = false;

        protected EntityStatCompo EntityStatCompo;
        protected float _damage;
        protected float _cooldown;
        protected float _lastAttackTime = -999f;
        protected PoolManagerMono _poolManager;

        public override void Initialize(TowerDataSO towerData, bool isCombatStage)
        {
            base.Initialize(towerData, isCombatStage);

            _poolManager = FindAnyObjectByType<PoolManagerMono>();
            
            EntityStatCompo = GetCompo<EntityStatCompo>(true);
            
            StatSO stat = EntityStatCompo.GetStat(cooldownStat);
            _cooldown = stat.BaseValue;
            stat.OnValueChanged += HandleCooldownChanged;

            stat = EntityStatCompo.GetStat(damageStat);
            _damage = stat.BaseValue;
            
            EntityAnimatorTrigger animTrigger = GetCompo<EntityAnimatorTrigger>();
            animTrigger.OnDamageCastTrigger += HandleAttack;
        }

        private void OnDestroy()
        {
            StatSO stat = EntityStatCompo.GetStat(cooldownStat);
            _cooldown = stat.BaseValue;
            stat.OnValueChanged -= HandleCooldownChanged;
            
            EntityAnimatorTrigger animTrigger = GetCompo<EntityAnimatorTrigger>();
            animTrigger.OnDamageCastTrigger -= HandleAttack;
        }

        private void HandleCooldownChanged(StatSO stat, float prev, float current) => _cooldown = current;

        public override void Spawn()
        {
        }

        protected override void Update()
        {
            base.Update();
            
            if(CanAttack == false || _lastAttackTime + _cooldown > Time.time) return;
            
            OnAttackEvent?.Invoke();
            HandleAttack();
            _lastAttackTime = Time.time;
        }

        protected abstract void HandleAttack();
    }
}