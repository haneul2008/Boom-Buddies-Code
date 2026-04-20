using System;
using Code.Entities;
using Code.Stats;
using UnityEngine;

namespace Code.Combat
{
    public class SphereDamageCaster : DamageCaster, IAfterInit
    {
        [SerializeField] private bool useStat;
        [SerializeField] private StatSO damageStat;
        [SerializeField, Range(0.5f, 3f)] private float castRadius = 1f;
        [SerializeField, Range(0, 1f)] private float casterInterpolation = 0.5f;
        [SerializeField, Range(0, 5f)] private float castingRange = 1f;
        [SerializeField] private int detectCnt = 1;

        private EntityStatCompo _entityStatCompo;
        private RaycastHit[] _res;

        public void AfterInit()
        {
            _res = new RaycastHit[detectCnt];
            
            if(useStat == false) return;
            
            _entityStatCompo = _owner.GetCompo<EntityStatCompo>(true);
            StatSO stat = _entityStatCompo.GetStat(damageStat);
            stat.OnValueChanged += HandleDamageChanged;
            _damage = stat.BaseValue;
        }

        private void OnDestroy()
        {
            if(useStat == false) return;

            StatSO stat = _entityStatCompo.GetStat(damageStat);
            stat.OnValueChanged -= HandleDamageChanged;
        }
        
        private void HandleDamageChanged(StatSO stat, float prev, float current) => _damage = current;

        public override void CastDamage(Vector3 position, Vector3 direction)
        {
            Vector3 startPosition = position + direction * -casterInterpolation * 2;

            int cnt = Physics.SphereCastNonAlloc(
                startPosition,
                castRadius,
                direction,
                _res,
                castingRange,
                whatIsTarget
                );

            for (int i = 0; i < cnt; ++i)
            {
                RaycastHit hit = _res[i];
                
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(_damage, hit.point, hit.normal, _owner);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 startPosition = transform.position + transform.forward * -casterInterpolation * 2;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, castRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPosition + transform.forward * castingRange, castRadius);
        }
#endif
    }
}