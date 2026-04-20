using System;
using Code.EventSystems;
using UnityEngine;

namespace Code.Shops
{
    public class ShopElement : MonoBehaviour
    {
        [SerializeField] protected GameEventChannelSO uiChannel;

        protected virtual void Awake()
        {
            uiChannel.AddListener<ShopToggleEvent>(HandleShopToggle);
        }

        private void Start()
        {
            SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            uiChannel.RemoveListener<ShopToggleEvent>(HandleShopToggle);
        }

        protected virtual void HandleShopToggle(ShopToggleEvent evt)
        {
            SetActive(evt.isActive);
        }

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}