using System;
using Code.EventSystems;
using Code.Managers;
using HNLib.Dependencies;
using TMPro;
using UnityEngine;

namespace Code.UI.Gold
{
    public class GoldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;
        
        [Inject] private GoldManager _goldManager;
        [Inject] private SaveManager _saveManager;

        private void Awake()
        {
            _saveManager.OnDataLoaded += HandleDataLoaded;
            GoldEvents.GoldChangeEvent.onChangeGold += HandleGoldChange;
        }

        private void Start()
        {
            HandleDataLoaded();
        }

        private void HandleDataLoaded()
        {
            goldText.text = _goldManager.CurrentGold.ToString();
        }

        private void OnDestroy()
        {
            _saveManager.OnDataLoaded -= HandleDataLoaded;
            GoldEvents.GoldChangeEvent.onChangeGold -= HandleGoldChange;
        }

        private void HandleGoldChange(bool isSuccess)
        {
            if(isSuccess)
                goldText.text = _goldManager.CurrentGold.ToString();
        }
    }
}