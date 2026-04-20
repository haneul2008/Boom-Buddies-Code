using UnityEngine;

namespace Code.Gravity
{
    public interface IGravity
    {
        public float GravityScale { get; }
        public Rigidbody Rigid { get; }
        public bool IsGround { get; }
        public float VerticalVelocity { get; }
        public void SetVerticalVelocity(float velocity);
    }
}