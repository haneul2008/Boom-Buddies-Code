using Code.Towers.Impl.Projectile;
using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Towers.Impl
{
    public class BombTower : AttackTower
    {
        [SerializeField] private PoolItemSO bombData;
        
        protected override void HandleAttack()
        {
            Bomb spawnedBomb = _poolManager.Pop<Bomb>(bombData);
            spawnedBomb.Initialize(fireTrm.position, _damage, rotationTrm.forward);
        }
    }
}