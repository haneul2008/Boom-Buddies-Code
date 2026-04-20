using Code.Entities;
using Code.FSM;

namespace Code.Towers.FSM
{
    public class NonAttackTowerIdleState : EntityState
    {
        public NonAttackTowerIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
    }
}