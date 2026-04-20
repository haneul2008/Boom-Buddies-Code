using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Towers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private Transform selectBarTrm;
        [SerializeField] private ShopSelectBtn selectBtn;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private SpawnableType initContent;

        private Dictionary<SpawnableType, ShopContentUI> _contentUiPairs = new Dictionary<SpawnableType, ShopContentUI>();
        private ShopContentUI _currentContent;

        private void Awake()
        {
            InitSelectBtn();
            InitContentUIs();

            uiChannel.AddListener<ShopToggleEvent>(HandleShopToggle);
            uiChannel.AddListener<ShopContentSelectEvent>(HandleContentSelect);
        }

        private void InitSelectBtn()
        {
            foreach (object contentEnum in Enum.GetValues(typeof(SpawnableType)))
            {
                Instantiate(selectBtn, selectBarTrm).Initialize((SpawnableType)contentEnum);
            }
        }

        private void InitContentUIs()
        {
            _contentUiPairs = GetComponentsInChildren<ShopContentUI>().ToDictionary(
                ui => ui.ContentEnum,
                ui => ui
            );

            _contentUiPairs.Values.ToList().ForEach(ui => ui.SetActive(false));
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<ShopToggleEvent>(HandleShopToggle);
            uiChannel.RemoveListener<ShopContentSelectEvent>(HandleContentSelect);
        }

        private void HandleShopToggle(ShopToggleEvent evt)
        {
            if (evt.isActive)
            {
                SpawnableType targetEnum = _currentContent == null ? initContent : _currentContent.ContentEnum;
                uiChannel.RaiseEvent(UIEvents.ShopContentSelectEvent.Initializer(targetEnum));
            }
        }

        private void HandleContentSelect(ShopContentSelectEvent evt)
        {
            ShopContentUI nextUI = _contentUiPairs[evt.contentEnum];
            
            if(nextUI == _currentContent) return;
            
            _currentContent?.Exit();
            _currentContent = nextUI;
            _currentContent?.Enter();
        }
    }
}