using System;
using Code.Core;
using Code.EventSystems;
using Code.Upgrade;
using UnityEngine;

namespace Code.Managers
{
    public class UpgradeManager : MonoBehaviour, IOnceManager
    {
        public int Priority => 0;
        
        [SerializeField] private GameEventChannelSO upgradeChannel;
        [SerializeField] private GameEventChannelSO goldChannel;

        private IUpgradeable _currentUpgradeable;
        private UpgradeDataSO _currentUpgrade;

        public void Initialize()
        {
            upgradeChannel.AddListener<TryUpgradeEvent>(HandleTryUpgrade);
        }

        private void OnDestroy()
        {
            upgradeChannel.RemoveListener<TryUpgradeEvent>(HandleTryUpgrade);
        }

        private void HandleTryUpgrade(TryUpgradeEvent evt)
        {
            _currentUpgradeable = evt.upgradeable;

            if (_currentUpgradeable is null) return;

            UpgradeDataListSO upgradeList = _currentUpgradeable.MyUpgrade;

            _currentUpgrade = upgradeList.GetUpgrade(_currentUpgradeable.UpgradeLevel);

            if (_currentUpgrade is null)
            {
                evt.onSuccess?.Invoke(false);
            }
            else
            {
                int requiredGold = -_currentUpgrade.requiredGold;
                GoldEvents.GoldChangeEvent.onChangeGold += HandleGoldChange;
                goldChannel.RaiseEvent(GoldEvents.GoldChangeEvent.Initializer(requiredGold));
            }
        }

        private void HandleGoldChange(bool isSuccess)
        {
            if (isSuccess)
            {
                _currentUpgradeable.UpgradeLevel++;
                _currentUpgradeable.Upgrade(_currentUpgrade);
            }
            
            GoldEvents.GoldChangeEvent.onChangeGold -= HandleGoldChange;
            UpgradeEvents.TryUpgradeEvent.onSuccess?.Invoke(isSuccess);
        }
    }
}