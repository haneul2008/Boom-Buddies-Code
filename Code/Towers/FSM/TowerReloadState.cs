using Code.Entities;
using UnityEngine;

namespace Code.Towers.FSM
{
    public class TowerReloadState : TowerState
    {
        public TowerReloadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();

            if (_isTriggerCall)
            {
                _tower.ChangeState("IDLE");
                _tower.lastAttackTime = Time.time;
            }
        }
    }
}