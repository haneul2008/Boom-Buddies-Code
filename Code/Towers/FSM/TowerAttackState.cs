using Code.Entities;
using Code.Extension;
using UnityEngine;

namespace Code.Towers.FSM
{
    public class TowerAttackState : TowerState
    {
        public TowerAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _tower.IsAutoRotate = false;

            Vector3 dir = (_tower.target.transform.position - _tower.transform.position).RemoveY();
            _tower.Look(dir);
        }

        public override void Update()
        {
            base.Update();
            
            if(_isTriggerCall)
                _tower.ChangeState("RELOAD");
        }

        public override void Exit()
        {
            base.Exit();

            _tower.IsAutoRotate = true;
        }
    }
}