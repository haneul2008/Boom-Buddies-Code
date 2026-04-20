using Code.Towers.Impl.Projectile;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Towers.Impl
{
    public class MortarTower : AttackTower
    {
        [SerializeField] private PoolItemSO cannonBallData;
        
        protected override void HandleAttack()
        {
            CannonBall spawnedCannonBall = _poolManager.Pop<CannonBall>(cannonBallData);
            spawnedCannonBall.Initialize(fireTrm.position, _damage, rotationTrm.forward);
        }
    }
}