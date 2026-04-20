using Code.Entities;
using UnityEngine;

namespace Code.Units.FSM
{
    public class UnitDeadState : UnitState
    {
        public UnitDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            _movement.SetStop(true);
        }

        public override void Update()
        {
            base.Update();

            if (_isTriggerCall)
            {
                _unit.OnFinalDead?.Invoke();
                GameObject.Destroy(_unit.gameObject);
            }
        }
    }
}