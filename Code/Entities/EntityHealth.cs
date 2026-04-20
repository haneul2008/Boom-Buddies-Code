using System;
using Code.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Entities
{
    public class EntityHealth : MonoBehaviour, IEntityComponent, IAfterInit
    {
        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        
        [SerializeField] private StatSO healthStat;

        public UnityEvent<float> OnHit;
        public UnityEvent<Entity> OnDead;

        private Entity _entity;

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void AfterInit()
        {
            EntityStatCompo entityStatCompo = _entity.GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(healthStat);
            stat.OnValueChanged += HandleHealthStatChanged;

            MaxHealth = stat.BaseValue;
            CurrentHealth = MaxHealth;
        }

        private void OnDestroy()
        {
            EntityStatCompo entityStatCompo = _entity.GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(healthStat);
            stat.OnValueChanged -= HandleHealthStatChanged;
        }

        public virtual void TakeDamage(float damage)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            OnHit?.Invoke(CurrentHealth);

            if (Mathf.Approximately(CurrentHealth, 0))
            {
                _entity.IsDead = true;
                OnDead?.Invoke(_entity);
            }
        }

        protected virtual void HandleHealthStatChanged(StatSO stat, float prev, float current)
        {
            MaxHealth = current;
            CurrentHealth *= current / prev;
        }
    }
}