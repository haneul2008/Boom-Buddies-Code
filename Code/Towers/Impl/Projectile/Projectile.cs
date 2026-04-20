using System;
using Code.Entities;
using Code.Gravity;
using Code.Managers;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Code.Towers.Impl.Projectile
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Projectile : MonoBehaviour, IGravity, IPoolable
    {
        public UnityEvent OnCollisionEvent;

        [field: SerializeField] public Rigidbody Rigid { get; private set; }
        [field: SerializeField] public float GravityScale { get; private set; } = -9.8f;
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }

        [SerializeField] protected DamageCaster damageCaster;
        [SerializeField] protected float speed;
        [SerializeField] protected float minGravity = -8f;

        public GameObject GameObject => gameObject;
        public float VerticalVelocity { get; private set; }
        public bool IsGround => false;

        protected float _damage;
        protected Vector3 _dir;
        protected Pool _myPool;

        public virtual void Initialize(Vector3 pos, float damage, Vector3 dir)
        {
            GravityManager gravityManager = CreateOnceManager.Instance.GetManager<GravityManager>();
            gravityManager.AddGravity(this);

            damageCaster.SetDamage(damage);

            VerticalVelocity = -0.03f;
            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(dir);
            
            _damage = damage;
            _dir = dir;
            
            damageCaster.Initialize(null);
            if (damageCaster is IAfterInit afterInit)
                afterInit.AfterInit();
        }

        private void OnDestroy()
        {
            GravityManager gravityManager = CreateOnceManager.Instance.GetManager<GravityManager>();
            gravityManager.RemoveGravity(this);
        }

        public virtual void Update()
        {
        }

        public virtual void Attack()
        {
            damageCaster.CastDamage(damageCaster.transform.position, transform.forward);
        }

        public virtual void SetVerticalVelocity(float velocity)
        {
            VerticalVelocity = Mathf.Clamp(velocity, minGravity, 0f);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Attack();

            OnCollisionEvent?.Invoke();
        }

        public virtual void SetUp(Pool pool)
        {
            _myPool = pool;
        }

        public virtual void ResetItem()
        {
        }
    }
}