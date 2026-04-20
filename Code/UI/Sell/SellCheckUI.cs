using System;
using Code.Core;
using Code.EventSystems;
using Code.Upgrade;
using TMPro;
using UnityEngine;

namespace Code.UI.Sell
{
    public class SellCheckUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO goldChannel;
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private TextMeshProUGUI goldText;

        private ISpawnable _currentSpawnable;
        private IUpgradeable _currentUpgradeable;
        private int _totalGold;
        
        private void Awake()
        {
            uiChannel.AddListener<PopUpSellCheckUIEvent>(HandlePopUpUI);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<PopUpSellCheckUIEvent>(HandlePopUpUI);
        }

        private void HandlePopUpUI(PopUpSellCheckUIEvent evt)
        {
            _currentSpawnable = evt.spawnable;
            _currentUpgradeable = evt.upgradeable;
                
            SpawnableDataSO spawnableData = evt.spawnable.SpawnableData;
            UpgradeDataListSO upgradeData = evt.upgradeable?.MyUpgrade;

            _totalGold = spawnableData.requiredGold;

            if (upgradeData is not null)
            {
                for (int i = 0; i < evt.upgradeable.UpgradeLevel; ++i)
                {
                    UpgradeDataSO data = upgradeData.GetUpgrade(i);
                    _totalGold += data.requiredGold;
                }
            }

            goldText.text = _totalGold.ToString();
            
            gameObject.SetActive(true);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
        }

        public void Sell()
        {
            goldChannel.RaiseEvent(GoldEvents.GoldChangeEvent.Initializer(_totalGold));
            systemChannel.RaiseEvent(SystemEvents.SpawnableSellEvent.Initializer(_currentSpawnable, _currentUpgradeable));
            gameObject.SetActive(false);
        }
    }
}