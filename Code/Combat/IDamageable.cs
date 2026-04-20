using Code.Entities;
using UnityEngine;

namespace Code.Combat
{
    public interface IDamageable
    {
        public void ApplyDamage(float damage, Vector3 hitPoint, Vector3 normal, Entity dealer);
    }
}