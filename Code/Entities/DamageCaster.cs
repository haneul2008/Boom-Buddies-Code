using UnityEngine;

namespace Code.Entities
{
    public abstract class DamageCaster : MonoBehaviour, IEntityComponent
    {
        [SerializeField] protected LayerMask whatIsTarget;

        protected Entity _owner;
        protected float _damage;

        public virtual void Initialize(Entity owner)
        {
            _owner = owner;
        }
        
        public void SetDamage(float damage) => _damage = damage;

        public abstract void CastDamage(Vector3 position, Vector3 direction);
    }
}