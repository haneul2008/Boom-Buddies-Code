using Code.Entities;
using UnityEngine;

namespace Code.Units.FSM
{
    public class UnitAttackWaitState : UnitState
    {
        private const float TargetResetDelay = 3f;
        
        private readonly float _cooldown;
        private float _enterTime;
        
        public UnitAttackWaitState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            EntityStatCompo entityStatCompo = _unit.GetCompo<EntityStatCompo>(true);
            _cooldown = entityStatCompo.GetStat(_unit.CooldownStat).BaseValue;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log(_unit.prevTarget);
            _enterTime = Time.time;
        }

        public override void Update()
        {
            base.Update();

            if (_movement.IsArrived && _unit.lastAttackTime + _cooldown < Time.time)
            {
                _unit.ChangeState("ATTACK");
            }
            else if(_enterTime + TargetResetDelay < Time.time)
            {
                _unit.ChangeState("MOVE");
            }
        }
    }
}