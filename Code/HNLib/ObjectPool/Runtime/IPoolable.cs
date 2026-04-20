using UnityEngine;

namespace HNLib.ObjectPool.Runtime
{
    public interface IPoolable
    {
        public PoolItemSO PoolItem { get; }
        public GameObject GameObject { get; }

        public void SetUp(Pool pool);
        public void ResetItem();
    }
}