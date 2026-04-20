using Code.Entities;
using Code.Extension;
using Code.Managers;
using Code.Towers;
using Code.Walls;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Units.FSM
{
    public class UnitMoveState : UnitState
    {
        protected const float TargetResetTime = 8f;

        protected readonly TowerManager _towerManager;
        protected readonly WallManager _wallManager;
        protected readonly NavMeshPath _path;
        protected Tower _targetTower;
        protected float _moveStartTime;

        public UnitMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _towerManager = CreateOnceManager.Instance.GetManager<TowerManager>();
            _wallManager = CreateOnceManager.Instance.GetManager<WallManager>();
            _path = new NavMeshPath();
        }

        public override void Enter()
        {
            base.Enter();

            _targetTower = null;
            _moveStartTime = Time.time;

            bool unitValid = _unit.target != null && _unit.target.IsDead == false;
            bool isSameTarget = _unit.prevTarget == _unit.target;
            
            _movement.SetStop(false);

            if (unitValid && isSameTarget) return;
            
            DecideTarget();
            CheckCanMoveComplete();
            _unit.prevTarget = _unit.target;
        }

        public override void Update()
        {
            base.Update();

            if (_movement.IsArrived && _movement.IsStop == false)
            {
                _movement.SetStop(true);
                _unit.ChangeState("ATTACKWAIT");
                return;
            }

            bool isResetTarget = _moveStartTime + TargetResetTime < Time.time;

            if (isResetTarget)
            {
                _unit.ChangeState("MOVE", true);
            }
        }

        private void CheckCanMoveComplete()
        {
            if (_targetTower == null) return;

            Vector3 targetPos = _unit.target.transform.position;

            bool pathFound = NavMesh.CalculatePath(_unit.transform.position, targetPos, NavMesh.AllAreas, _path);
            if (pathFound == false || _path.status != NavMeshPathStatus.PathComplete)
            {
                Vector3Int towerPos = _targetTower.transform.position.ToVector3Int();
                Vector3 unitPos = _unit.transform.position;
                Wall targetWall = _wallManager.GetOptimalWall(GetGridPos(unitPos), towerPos);
                Debug.Log(targetWall);

                if (targetWall == null || _unit.target == targetWall)
                {
                    _movement.SetDestination(_unit.target.transform.position);
                    return;
                }

                _unit.target = targetWall;
                Debug.Log(_unit.target);
            }

            _movement.SetDestination(_unit.target.transform.position);
        }

        protected virtual void DecideTarget()
        {
            foreach (Tower tower in _towerManager.SpawnedTowerList)
            {
                _targetTower ??= tower;

                Vector3 unitPos = _entity.transform.position.RemoveY();
                Vector3 targetPos = _targetTower.transform.position.RemoveY();
                Vector3 currentTowerPos = tower.transform.position.RemoveY();

                float targetDistance = Vector3.Distance(unitPos, targetPos);
                float currentDistance = Vector3.Distance(unitPos, currentTowerPos);

                if (_targetTower.Priority < tower.Priority ||
                    (_targetTower.Priority == tower.Priority && targetDistance > currentDistance))
                {
                    _targetTower = tower;
                }
            }

            if (_targetTower == null || _unit.target == _targetTower || _targetTower.IsDead) return;

            _unit.target = _targetTower;
        }

        private Vector3Int GetGridPos(Vector3 worldPosition)
        {
            int y = Mathf.FloorToInt(worldPosition.y / 2f) * 2;
            int z = Mathf.FloorToInt(worldPosition.z / 2f) * 2;
            int x = Mathf.FloorToInt(worldPosition.x / 2f) * 2;

            return new Vector3Int(x, y, z);
        }
    }
}