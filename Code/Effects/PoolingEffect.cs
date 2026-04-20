using DG.Tweening;
using HNLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.Effects
{
    public class PoolingEffect : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        public GameObject GameObject => gameObject;
        public IPlayableVFX PlayableVFX { get; private set; }

        private Pool _myPool;
        [SerializeField] private GameObject effectObject;
        
        public void SetUp(Pool pool)
        {
            _myPool = pool;
            PlayableVFX = effectObject.GetComponent<IPlayableVFX>(); 
        }

        public void ResetItem()
        {
            PlayableVFX.StopVFX();
        }

        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            PlayableVFX.PlayVFX(position, rotation);
            DOVirtual.DelayedCall(PlayableVFX.Duration, () => _myPool.Push(this));
        }

        private void OnValidate()
        {
            if (effectObject == null) return;
            PlayableVFX = effectObject.GetComponent<IPlayableVFX>();
            if (PlayableVFX == null)
            {
                Debug.LogWarning($"Input object {effectObject.name} does not have IPoolable component");
                effectObject = null;
            }
        }
    }
}