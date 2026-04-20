using Code.Entities;
using Code.Extension;
using Code.Towers;
using Code.Towers.Impl;
using UnityEngine;

namespace Code.Units.FSM
{
    public class SkeletonMoveState : UnitMoveState
    {
        public SkeletonMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        protected override void DecideTarget()
        {
            int targetHealth = 0;
            
            foreach (Tower tower in _towerManager.SpawnedTowerList)
            {
                if(_towerManager.SpawnedTowerList.Count > 1 && tower.GetType() == typeof(CoreTower)) continue;
                
                _targetTower ??= tower;

                Vector3 unitPos = _entity.transform.position.RemoveY();
                Vector3 targetPos = _targetTower.transform.position.RemoveY();
                Vector3 currentTowerPos = tower.transform.position.RemoveY();

                float targetDistance = Vector3.Distance(unitPos, targetPos);
                float currentDistance = Vector3.Distance(unitPos, currentTowerPos);

                int health = (int)tower.GetCompo<EntityHealth>().CurrentHealth;
                
                if (targetHealth < health ||
                    (targetHealth == health && targetDistance > currentDistance))
                {
                    targetHealth = health;
                    _targetTower = tower;
                }
            }
            
            if (_targetTower == null || _unit.target == _targetTower) return;

            _unit.target = _targetTower;
            _movement.SetDestination(_targetTower.transform.position);
        }
    }
}