using System;
using HNLib.Dependencies;
using UnityEngine;

namespace HNLib.ObjectPool.Runtime
{
    [Provide]
    public class PoolManagerMono : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private PoolManagerSO poolManager;

        private void Awake()
        {
            poolManager.Initialize(transform);
        }

        public T Pop<T>(PoolItemSO item) where T : IPoolable
        {
            return (T)poolManager.Pop(item);
        }

        public void Push(IPoolable target)
        {
            poolManager.Push(target);
        }
    }
}