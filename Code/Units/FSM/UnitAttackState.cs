using Code.Entities;
using UnityEngine;

namespace Code.Units.FSM
{
    public class UnitAttackState : UnitState
    {
        public UnitAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            _movement.SetStop(true);
            
            if(_unit.target == null) return;
            
            Vector3 dir = _unit.target.transform.position - _unit.transform.position;
            _unit.transform.rotation = Quaternion.LookRotation(dir);
        }

        public override void Update()
        {
            if (_isTriggerCall)
            {
                _unit.ChangeState("MOVE");
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            _unit.lastAttackTime = Time.time;
        }
    }
}