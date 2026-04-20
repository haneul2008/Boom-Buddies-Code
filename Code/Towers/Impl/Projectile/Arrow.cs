using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Towers.Impl.Projectile
{
    public class Arrow : Projectile
    {
        public void Initialize(Vector3 pos, float damage, Vector3 dir, float angleAdder)
        {
            base.Initialize(pos, damage, dir);
            
            Vector3 eulerAngle = transform.eulerAngles;
            transform.eulerAngles = new Vector3(eulerAngle.x, eulerAngle.y + angleAdder, eulerAngle.z);
            _dir = transform.forward;
        }

        public override void Update()
        {
            base.Update();
            
            Rigid.linearVelocity = _dir * speed;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            
            _myPool.Push(this);
        }
    }
}