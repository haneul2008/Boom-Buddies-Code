using System;
using UnityEngine;

namespace Code.Entities
{
    public class EntityAnimatorTrigger : MonoBehaviour, IEntityComponent
    {
        public Action OnAnimationEndTrigger;
        public Action OnDamageCastTrigger;
        //public Action OnAnimationEventTrigger;
        public Action OnAttackVFXTrigger;
        
        private Entity _entity;
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        private void AnimationEnd() => OnAnimationEndTrigger?.Invoke();
        private void PlayAttackVFX() => OnAttackVFXTrigger?.Invoke();
        private void DamageCast() => OnDamageCastTrigger?.Invoke();
    }
}