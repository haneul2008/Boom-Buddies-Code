using System;
using Code.Core;
using Code.EventSystems;
using Code.Towers;
using Code.Upgrade;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI.Upgrades
{
    public class SpawnableUpgradeBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<bool> OnPointerEnterEvent;
        public event Action OnUpgradeSuccess;

        [SerializeField] private GameEventChannelSO upgradeChannel;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private float tweenDuration;

        private IUpgradeable _currentUpgradeable;
        private bool _isMax;
        private Tween _colorTween;

        public void SetUp(IUpgradeable upgradeable)
        {
            if (upgradeable == null) return;
            
            _currentUpgradeable = upgradeable;

            if (_currentUpgradeable == null)
            {
                gameObject.SetActive(false);
                return;
            }

            bool isMax = _currentUpgradeable.MyUpgrade.IsMax(_currentUpgradeable.UpgradeLevel);

            gameObject.SetActive(!isMax);
        }

        public void Upgrade()
        {
            if (_currentUpgradeable == null) return;

            UpgradeEvents.TryUpgradeEvent.onSuccess += HandleSuccess;
            upgradeChannel.RaiseEvent(UpgradeEvents.TryUpgradeEvent.Initializer(_currentUpgradeable));
        }

        private void HandleSuccess(bool isSuccess)
        {
            _colorTween?.Complete();

            Color targetColor = isSuccess ? Color.green : Color.red;
            _colorTween = upgradeText.DOColor(targetColor, tweenDuration).SetLoops(2, LoopType.Yoyo).SetUpdate(true);

            if (isSuccess)
            {
                OnUpgradeSuccess?.Invoke();

                if (_currentUpgradeable.MyUpgrade.IsMax(_currentUpgradeable.UpgradeLevel))
                    gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) => OnPointerEnterEvent?.Invoke(true);
        public void OnPointerExit(PointerEventData eventData) => OnPointerEnterEvent?.Invoke(false);
    }
}