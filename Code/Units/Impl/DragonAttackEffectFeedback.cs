using System;
using System.Collections;
using Code.Effects;
using Code.Feedbacks;
using DG.Tweening;
using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Units.Impl
{
    public class DragonAttackEffectFeedback : Feedback
    {
        [SerializeField] private Transform rotationTrm;
        [SerializeField] private Transform fireTrm;
        [SerializeField] private PoolItemSO fireEffect;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private int cnt;
        [SerializeField] private float delay;

        private WaitForSeconds _seconds;

        private void Awake()
        {
            _seconds = new WaitForSeconds(delay);
        }

        public override void PlayFeedback()
        {
            StartCoroutine(EffectPlayCoroutine());
        }

        private IEnumerator EffectPlayCoroutine()
        {
            for (int i = 0; i < cnt; ++i)
            {
                DragonAttackEffect effect = poolManager.Pop(fireEffect) as DragonAttackEffect;
                effect.Initialize(rotationTrm.forward, fireTrm.position, Quaternion.identity);
                
                yield return _seconds;
            }
        }

        public override void StopFeedback()
        {
            
        }
    }
}