using Code.Effects;
using Code.Extension;
using Code.Stats;
using Code.Towers.Impl.Projectile;
using Code.Upgrade;
using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Towers.Impl
{
    public class MissileTower : AttackTower
    {
        [SerializeField] private StatSO rangeStat;
        [SerializeField] private int detectCnt = 5;
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private PoolItemSO missileData;
        [SerializeField] private PoolItemSO missileFireEffect;
        [SerializeField] private float forwardRandomizeValue = 5f;

        private Collider[] _res;
        private MissileUpgradeDataSO _missileUpgradeData;
        private MissileTowerVisual _visual;

        public override void Initialize(TowerDataSO towerData, bool isCombatStage)
        {
            base.Initialize(towerData, isCombatStage);

            _res = new Collider[detectCnt];
            _visual = GetComponentInChildren<MissileTowerVisual>();
        }

        protected override void HandleAttack()
        {
            float range = EntityStatCompo.GetStat(rangeStat).Value;

            int missileCnt = _visual.FireTrmList.Count;
            
            int cnt = Physics.OverlapSphereNonAlloc(transform.position, range, _res, whatIsTarget);
            cnt = Mathf.Clamp(cnt, 0, missileCnt);
            if(cnt == 0) return;
            
            for (int i = 0; i < missileCnt; ++i)
            {
                Transform colliderTrm = null;

                if (i >= cnt) 
                    colliderTrm = _res[cnt - 1].transform;
                else
                    colliderTrm = _res[i].transform;
                
                Vector3 dir = (colliderTrm.position - transform.position).RemoveY();
                
                float randomAngle = Random.Range(-forwardRandomizeValue, forwardRandomizeValue);
                Vector3 randomForward = Quaternion.AngleAxis(randomAngle, Vector3.up) * rotationTrm.forward;

                Transform fireTrm = _visual.FireTrmList[i];
                
                Missile missile = _poolManager.Pop<Missile>(missileData);
                missile.Initialize(fireTrm.position, _damage, randomForward, dir, colliderTrm);

                PoolingEffect fireEffect = _poolManager.Pop<PoolingEffect>(missileFireEffect);
                fireEffect.PlayableVFX.PlayVFX(fireTrm.position, Quaternion.identity);
            }
        }

        protected override void SetUpgradeVisual(UpgradeDataSO upgradeData)
        {
            _missileUpgradeData = upgradeData as MissileUpgradeDataSO;

            if(_missileUpgradeData is null) return;
            
            _visual.ChangeVisual(_missileUpgradeData.visualPrefab);
        }
    }
}