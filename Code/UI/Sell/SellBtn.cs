using Code.Core;
using Code.EventSystems;
using UnityEngine;

namespace Code.UI.Sell
{
    public class SellBtn : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;

        private ISpawnable _currentSpawnable;
        private IUpgradeable _currentUpgradeable;
        
        public void SetUp(ISpawnable spawnable, IUpgradeable upgradeable)
        {
            _currentSpawnable = spawnable;
            _currentUpgradeable = upgradeable;
        }
        
        public void OnClick()
        {
            if(_currentSpawnable == null) return;
            
            uiChannel.RaiseEvent(UIEvents.PopUpSellCheckUIEvent.Initializer(_currentSpawnable, _currentUpgradeable));
        }
    }
}