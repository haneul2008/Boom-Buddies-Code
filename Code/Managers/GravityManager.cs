using System;
using System.Collections.Generic;
using Code.Gravity;
using UnityEngine;

namespace Code.Managers
{
    public class GravityManager : MonoBehaviour
    {
        private readonly List<IGravity> _gravities = new List<IGravity>();

        public void AddGravity(IGravity gravity)
        {
            if (_gravities.Contains(gravity) == false)
                _gravities.Add(gravity);
        }

        public void RemoveGravity(IGravity gravity)
        {
            if (_gravities.Contains(gravity))
                _gravities.Remove(gravity);
        }

        public void FixedUpdate()
        {
            foreach (IGravity gravity in _gravities)
            {
                if (gravity.IsGround)
                    gravity.SetVerticalVelocity(-0.03f);
                else
                    gravity.SetVerticalVelocity(gravity.VerticalVelocity + gravity.GravityScale * Time.fixedDeltaTime);
                
                gravity.Rigid.AddForce(new Vector3(0, gravity.VerticalVelocity, 0), ForceMode.Impulse);
            }
        }
    }
}