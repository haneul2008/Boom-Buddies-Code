using System;
using Code.Effects;
using DG.Tweening;
using HNLib.ObjectPool.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Units.Impl
{
    public class DragonAttackEffect : PoolingEffect
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private float forwardRandomizeValue;
        [SerializeField] private float speed;

        private Vector3 _dir;
        
        public void Initialize(Vector3 dir, Vector3 pos, Quaternion rotation)
        {
            PlayVFX(pos, rotation);
            
            float randomAngle = Random.Range(-forwardRandomizeValue, forwardRandomizeValue);
            Vector3 randomForward = Quaternion.AngleAxis(randomAngle, Vector3.up) * dir;

            _dir = randomForward;

            DOVirtual.DelayedCall(PlayableVFX.Duration + 0.2f, () =>
            {
                poolManager.Push(this);
            });
        }

        private void Update()
        {
            transform.position += _dir * (speed * Time.deltaTime);
        }
    }
}