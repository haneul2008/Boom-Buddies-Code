using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Stats
{
    public enum StatEnum
    {
        Health,
        Damage,
        Range,
        Cooldown,
        GoldProductionAmount,
        GoldStorage
    }

    public class StatUI : MonoBehaviour
    {
        public StatUIDataSO StatUIData { get; private set; }
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI upgradeText;

        private RectTransform _iconRectTrm;

        public void SetUp(StatUIDataSO statUIData)
        {
            _iconRectTrm = iconImage.transform as RectTransform;
            
            iconImage.sprite = statUIData.icon;
            _iconRectTrm.sizeDelta = statUIData.iconSize;
            titleText.text = statUIData.uiTitle;
            StatUIData = statUIData;
        }
        
        public void SetEnable(bool isEnable) => gameObject.SetActive(isEnable);

        public void SetStatText(string text)
        {
            statText.text = text;
            upgradeText.enabled = false;
        }

        public void SetUpgradeText(bool isEnable, float value)
        {
            if (isEnable)
            {
                if (value > 0)
                    upgradeText.text = $"+{value}";
                else
                    upgradeText.text = value.ToString();
            }

            upgradeText.enabled = isEnable;
        }
    }
}