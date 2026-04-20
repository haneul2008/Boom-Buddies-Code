using Code.Towers.Impl.Projectile;
using Code.Upgrade;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Towers.Impl
{
    public class CrossbowTower : AttackTower
    {
        [SerializeField] private PoolItemSO arrowData;

        private CrossbowUpgradeDataSO _crossbowUpgradeData;

        public override void Upgrade(UpgradeDataSO upgradeData)
        {
            base.Upgrade(upgradeData);

            _crossbowUpgradeData = upgradeData as CrossbowUpgradeDataSO;
        }

        protected override void HandleAttack()
        {
            float arrowCnt = _crossbowUpgradeData?.arrowCnt ?? 1;
            float spreadAdder = _crossbowUpgradeData?.arrowSpreadAngle ?? 0;
            
            float angleAdder = -(spreadAdder * (arrowCnt / 2f));
            
            for (int i = 0; i < arrowCnt; ++i)
            {
                Arrow spawnedArrow = _poolManager.Pop<Arrow>(arrowData);
                spawnedArrow.Initialize(fireTrm.position, _damage, rotationTrm.forward, angleAdder);
                angleAdder += spreadAdder;
            }
        }
    }
}