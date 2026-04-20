using Code.Entities;
using Code.FSM;
using UnityEngine;

namespace Code.Units.FSM
{
    public abstract class UnitState : EntityState
    {
        protected Unit _unit;
        protected NavMovement _movement;
        
        public UnitState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _unit = entity as Unit;
            _movement = _unit.GetCompo<NavMovement>();
        }

        public override void Update()
        {
            base.Update();
            
            if (_unit.target == null || _unit.target.IsDead)
            {
                _unit.ChangeState("MOVE", true);
            }
        }
    }
}