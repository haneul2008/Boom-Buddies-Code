using System.Collections.Generic;
using Code.Core;
using Code.Entities;
using Code.Stats;
using Code.UI.Spawnable;
using Code.UI.Stats;
using Code.Units;
using TMPro;
using UnityEngine;

namespace Code.UI.Units
{
    public class UnitInfoPanel : SpawnableInfoPanel
    {
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI costText;
        
        private readonly Dictionary<UnitDataSO, UnitStatCompo> _statCompoPairs = new Dictionary<UnitDataSO, UnitStatCompo>();
        private UnitStatCompo _statCompo;
        
        public override void Initialize()
        {
            base.Initialize();
            
            upgradeBtn.gameObject.SetActive(false);
            upgradeGoldUI.gameObject.SetActive(false);
        }

        protected override void SetData(IUpgradeable upgradeable)
        {
            UnitDataSO targetData = _currentSpawnable.SpawnableData as UnitDataSO;
            
            if(targetData == null) return;

            if (_statCompoPairs.TryGetValue(targetData, out UnitStatCompo unitStatCompo))
                _statCompo = unitStatCompo;
            else
            {
                UnitStatCompo newStatCompo = ((MonoBehaviour)_currentSpawnable).GetComponentInChildren<UnitStatCompo>();
                _statCompoPairs.Add(targetData, newStatCompo);
                _statCompo = newStatCompo;
            }
            
            spawnableImage.sprite = _currentSpawnable.SpawnableData.spawnableSprite;
            nameText.text = $"{_currentSpawnable.SpawnableData.spawnableName}";
            descText.text = targetData.desc;
            costText.text = targetData.cost.ToString();

            SetStatUI(_statCompo);
        }

        protected override void SetStatUIElements(EntityStatCompo statCompo, StatSO stat, StatUI targetUI)
        {
            float overrideValue = _statCompo.GetOverrideStat(stat).GetBaseValue();
            targetUI?.SetStatText(overrideValue.ToString());
            targetUI?.SetEnable(true);
        }
    }
}