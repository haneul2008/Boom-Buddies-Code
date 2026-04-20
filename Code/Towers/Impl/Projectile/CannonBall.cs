using UnityEngine;

namespace Code.Towers.Impl.Projectile
{
    public class CannonBall : Projectile
    {
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