using Code.Entities;
using Code.Extension;
using UnityEngine;

namespace Code.Towers.Impl.Projectile
{
    public class Missile : Projectile
    {
        [SerializeField] private float moveForwardTime = 0.7f;

        private Entity _target;
        private Vector3 _forward;
        private float _firedTime;
        
        public void Initialize(Vector3 pos, float damage, Vector3 forward, Vector3 dir, Transform targetTrm)
        {
            base.Initialize(pos, damage, dir);
            _target = targetTrm.GetComponent<Entity>();
            _forward = forward;
            _firedTime = Time.time;
        }

        public override void Update()
        {
            base.Update();

            if (_firedTime + moveForwardTime > Time.time)
            {
                Rigid.linearVelocity = _forward * speed;
            }
            else
            {
                if (_target == null || _target.IsDead)
                {
                    _myPool.Push(this);
                    return;
                }
                
                Vector3 dir = (_target.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(dir);
                Rigid.linearVelocity = dir * speed;
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            
            if (other != null && other.gameObject == _target.gameObject)
            {
                Attack();
                _myPool.Push(this);
            }
        }
    }
}