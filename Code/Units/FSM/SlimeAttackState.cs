using Code.Entities;

namespace Code.Units.FSM
{
    public class SlimeAttackState : UnitAttackState
    {
        public SlimeAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
        
        public override void Update()
        {
            if (_isTriggerCall)
            {
                EntityHealth health = _entity.GetCompo<EntityHealth>();
                health.TakeDamage(health.MaxHealth);
            }
        }
    }
}