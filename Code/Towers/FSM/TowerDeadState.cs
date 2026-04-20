using Code.Entities;
using UnityEngine;

namespace Code.Towers.FSM
{
    public class TowerDeadState : TowerState
    {
        public TowerDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();

            if (_isTriggerCall)
            {
                _tower.OnFinalDeadEvent?.Invoke();
                GameObject.Destroy(_tower.gameObject);
            }
        }
    }
}