using System;
using System.Collections;
using Code.Effects;
using Code.Managers;
using HNLib.Dependencies;
using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Feedbacks
{
    public class PlayEffectFeedback : Feedback
    {
        [SerializeField] private PoolItemSO effect;
        [SerializeField] private Transform playTrm;

        private PoolManagerMono _poolManager;

        private void Awake()
        {
            _poolManager = FindAnyObjectByType<PoolManagerMono>();
        }

        public override void PlayFeedback()
        {
            PoolingEffect poolingEffect = _poolManager.Pop<PoolingEffect>(effect);
            poolingEffect.PlayVFX(playTrm.transform.position, Quaternion.identity);
        }

        public override void StopFeedback()
        {
            
        }
    }
}