using Code.Core;
using UnityEngine;

namespace Code.UI.Shops
{
    public abstract class ShopContentUI : MonoBehaviour
    {
        [field: SerializeField] public SpawnableType ContentEnum { get; private set; }

        public abstract void SetActive(bool isActive);

        public virtual void Enter()
        {
            SetActive(true);
        }

        public virtual void Exit()
        {
            SetActive(false);
        }
    }
}