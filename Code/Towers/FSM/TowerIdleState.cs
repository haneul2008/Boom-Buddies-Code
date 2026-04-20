using Code.Entities;
using Code.Extension;
using Code.Managers;
using Code.Units;
using UnityEngine;

namespace Code.Towers.FSM
{
    public class TowerIdleState : TowerState
    {
        private const float CheckInterval = 0.1f;

        private readonly int _unitLayer = 1 << LayerMask.NameToLayer("Unit");
        private readonly int _wallLayer = 1 << LayerMask.NameToLayer("Wall");
        private readonly UnitManager _unitManager;
        private readonly float _cooldown;
        private readonly float _range;
        private readonly Collider[] _res;
        private readonly Vector3 _raycastOffset = new Vector3(0, 0.5f, 0);
        private float _lastCheckTime = -999f;
        private bool _isUnitTargeting;
        private Entity _prevTarget;

        public TowerIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _unitManager = CreateOnceManager.Instance.GetManager<UnitManager>();

            TowerStatCompo entityStatCompo = _tower.GetCompo<TowerStatCompo>();
            TowerInfo towerInfo = entityStatCompo.GetInfo();
            _cooldown = towerInfo.cooldown ?? 0;
            _range = towerInfo.range ?? 0;

            _res = new Collider[10];
            
            bool unitValid = _tower.target != null && _tower.target.IsDead == false;
            bool isSameTarget = _prevTarget == _tower.target;

            _isUnitTargeting = unitValid && isSameTarget;
        }

        public override void Update()
        {
            base.Update();

            bool isCooldown = _tower.lastAttackTime + _cooldown < Time.time;

            if (isCooldown && _tower.target != null && _tower.IsRotating == false)
            {
                _tower.ChangeState("ATTACK");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(_isUnitTargeting) return;
            
            CheckUnits();
        }

        private void CheckUnits()
        {
            if (_lastCheckTime + CheckInterval > Time.time) return;

            int cnt = DetectUnitCnt();

            Collider target = null;
            float shortnessDistance = float.MaxValue;

            for (int i = 0; i < cnt; ++i)
            {
                Unit unit = _unitManager.GetUnit(_res[i]);

                if (unit is null || unit.IsPlaced == false || CantDetect(_res[i])) continue;

                target = TryDecideTarget(i, ref shortnessDistance, target);
            }

            _lastCheckTime = Time.time;
            _tower.target = _unitManager.GetUnit(target);
            _prevTarget = _tower.target;
        }

        private bool CantDetect(Collider target)
        {
            if (_tower.CanDetectWallOut) return false;

            Vector3 dir = (target.transform.position - _tower.transform.position).normalized.RemoveY();

            return Physics.Raycast(_tower.transform.position + _raycastOffset, dir, _range,
                _wallLayer);
        }

        private Collider TryDecideTarget(int i, ref float shortnessDistance, Collider target)
        {
            float distance = Vector3.Distance(_tower.transform.position.RemoveY(),
                _res[i].transform.position.RemoveY());

            if (distance < shortnessDistance)
            {
                shortnessDistance = distance;
                target = _res[i];
            }

            return target;
        }

        private int DetectUnitCnt()
        {
            return Physics.OverlapSphereNonAlloc(_tower.transform.position, _range, _res,
                _unitLayer);
        }
    }
}