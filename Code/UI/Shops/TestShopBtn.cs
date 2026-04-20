using Code.EventSystems;
using UnityEngine;

namespace Code.UI.Shops
{
    public class TestShopBtn : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        
        public void RaiseShopToggleEvent()
        {
            ShopToggleEvent evt = UIEvents.ShopToggleEvent;
            uiChannel.RaiseEvent(evt.Initializer(!evt.isActive));
        }
    }
}