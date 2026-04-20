using System;
using Code.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Entities
{
    public class EntityAttackCompo : MonoBehaviour, IEntityComponent
    {
        public UnityEvent OnAttackEvent;
        
        [SerializeField] private StatSO damageStat;
        
        private DamageCaster _damageCaster;
        private Entity _entity;
        private float _damage;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _damageCaster = entity.GetCompo<DamageCaster>(true);

            EntityStatCompo entityStatCompo = entity.GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(damageStat);
            stat.OnValueChanged += HandleDamageChange;
            _damage = stat.BaseValue;
            
            EntityAnimatorTrigger animTrigger = entity.GetCompo<EntityAnimatorTrigger>();
            animTrigger.OnDamageCastTrigger += HandleDamageCast;
        }

        private void OnDestroy()
        {
            EntityAnimatorTrigger animTrigger = _entity.GetCompo<EntityAnimatorTrigger>();
            animTrigger.OnDamageCastTrigger -= HandleDamageCast;
            
            EntityStatCompo entityStatCompo = _entity.GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(damageStat);
            stat.OnValueChanged -= HandleDamageChange;
        }

        private void HandleDamageChange(StatSO stat, float prev, float current) => _damage = current;

        private void HandleDamageCast()
        {
            OnAttackEvent?.Invoke();
            _damageCaster.CastDamage(_damageCaster.transform.position, _entity.transform.forward);
        }
    }
}