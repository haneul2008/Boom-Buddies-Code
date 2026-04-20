using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.References;
using Code.References.Text;
using Code.Stats;
using Code.Towers;
using Code.Towers.Impl;
using Code.UI.ETC;
using Code.UI.Gold;
using Code.UI.Sell;
using Code.UI.Stats;
using Code.UI.Upgrades;
using Code.Upgrade;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Spawnable
{
    public class SpawnableInfoPanel : MonoBehaviour
    {
        [Header("References")]
        [field: SerializeField]
        public List<SpawnableDataSO> TargetDataList { get; private set; }

        [SerializeField] private TextContainerSO textContainer;
        [SerializeField] private SpawnableStatListSO spawnableStatList;
        [SerializeField] private List<StatUIDataSO> statUiDataList;

        [Header("UI Setting")] [SerializeField]
        protected TextMeshProUGUI nameText;

        [SerializeField] protected Image spawnableImage;
        [SerializeField] protected SpawnableUpgradeBtn upgradeBtn;
        [SerializeField] protected UpgradeGoldUI upgradeGoldUI;
        [SerializeField] private SellBtn sellBtn;

        [Header("Tween Setting")] [SerializeField]
        private Vector2 hidePos;

        [SerializeField] private float activeDuration;

        protected ISpawnable _currentSpawnable;
        protected ISpawnable _prevSpawnable;
        private RectTransform _rectTrm;
        private Vector2 _originPos;
        private Tween _activeTween;
        private Entity _currentEntity;
        private List<StatUI> _statUIList;
        private StatUIContainer _statUIContainer;
        private List<StatEnum> _excludeStatEnums = new List<StatEnum>();

        public virtual void Initialize()
        {
            _rectTrm = transform as RectTransform;
            _originPos = _rectTrm.anchoredPosition;
            _rectTrm.anchoredPosition = hidePos;

            _statUIList = GetComponentsInChildren<StatUI>().ToList();
            _statUIContainer = new StatUIContainer(statUiDataList, _statUIList);

            upgradeBtn.OnPointerEnterEvent += HandleUpgradeBtnEnter;
            upgradeBtn.OnUpgradeSuccess += HandleUpgradeSuccess;

            for (int i = 0; i < _statUIList.Count; ++i)
                _statUIList[i].SetUp(statUiDataList[i]);
        }

        protected virtual void OnDestroy()
        {
            _activeTween?.Kill();

            upgradeBtn.OnPointerEnterEvent -= HandleUpgradeBtnEnter;
            upgradeBtn.OnUpgradeSuccess -= HandleUpgradeSuccess;
        }

        private void HandleUpgradeSuccess()
        {
            SetData(_currentEntity as IUpgradeable);
            HandleUpgradeBtnEnter(true);
        }

        private void HandleUpgradeBtnEnter(bool isEnter)
        {
            if (_currentSpawnable == null || _currentSpawnable is not IUpgradeable upgradeable) return;

            UpgradeDataSO nextUpgrade = upgradeable.MyUpgrade.GetUpgrade(upgradeable.UpgradeLevel);

            if (nextUpgrade == null) return;

            if (isEnter)
            {
                spawnableImage.sprite = nextUpgrade.upgradeSprite;
                
                foreach (UpgradeStat upgradeStat in nextUpgrade.upgrades)
                {
                    string enumText = textContainer.GetTextData(upgradeStat.targetStat.statName).text[0];
                    if (Enum.TryParse(enumText, out StatEnum statEnum))
                    {
                        _statUIContainer.GetStatUI(statEnum).SetUpgradeText(true, upgradeStat.modifyValue);
                    }
                }
            }
            else
            {
                if (upgradeable.UpgradeLevel - 1 == -1)
                    spawnableImage.sprite = _currentSpawnable.SpawnableData.spawnableSprite;
                else
                {
                    UpgradeDataSO currentUpgrade = upgradeable.MyUpgrade.GetUpgrade(upgradeable.UpgradeLevel - 1);
                    spawnableImage.sprite = currentUpgrade.upgradeSprite;
                }
                
                foreach (StatUI statUI in _statUIContainer.GetAllStatUI())
                    statUI.SetUpgradeText(false, 0);
            }
        }

        public virtual void SetActive(ISpawnable spawnable, bool isActive)
        {
            _prevSpawnable = _currentSpawnable;
            _currentSpawnable = spawnable;
            _currentEntity = _currentSpawnable as Entity;
            IUpgradeable upgradeable = _currentSpawnable as IUpgradeable;

            if (isActive && _currentEntity != null && _prevSpawnable != _currentSpawnable)
            {
                SetData(upgradeable);
                upgradeBtn.SetUp(upgradeable);

                if (spawnable is CoreTower)
                {
                    sellBtn.gameObject.SetActive(false);
                }
                else
                {
                    sellBtn.gameObject.SetActive(true);
                    sellBtn.SetUp(_currentSpawnable, upgradeable);
                }
            }

            SetTween(isActive);
        }

        protected virtual void SetData(IUpgradeable upgradeable)
        {
            SetStatUI(_currentEntity.GetCompo<EntityStatCompo>(true));

            int upgradeLevel = upgradeable.UpgradeLevel;
            bool isMax = upgradeable.MyUpgrade.IsMax(upgradeLevel);
            nameText.text = $"{_currentSpawnable.SpawnableData.spawnableName} Lv.{(isMax ? "Max" : upgradeLevel + 1)}";

            if (isMax)
            {
                upgradeGoldUI.SetText(false, 0);
                spawnableImage.sprite = upgradeable.MyUpgrade.upgradeList[^1].upgradeSprite;
            }
            else
            {
                UpgradeDataSO nextUpgrade = upgradeable.MyUpgrade.GetUpgrade(upgradeLevel);
                
                int requiredGold = nextUpgrade.requiredGold;
                upgradeGoldUI.SetText(true, requiredGold);

                if (upgradeLevel - 1 == -1)
                    spawnableImage.sprite = _currentSpawnable.SpawnableData.spawnableSprite;
                else
                {
                    UpgradeDataSO currentUpgrade = upgradeable.MyUpgrade.GetUpgrade(upgradeLevel - 1);
                    spawnableImage.sprite = currentUpgrade.upgradeSprite;
                }
            }
        }

        protected void SetStatUI(EntityStatCompo statCompo)
        {
            List<StatUI> statUIList = _statUIContainer.GetAllStatUI();

            foreach (StatUI statUI in statUIList)
                statUI.SetEnable(false);

            List<StatSO> stats = spawnableStatList.GetStatList(_currentSpawnable.SpawnableData);

            bool isAnotherSpawnable = _prevSpawnable?.GetType() != _currentSpawnable?.GetType();

            if (isAnotherSpawnable) SetExcludeEnums(stats);

            int excludeIndex = 0;

            foreach (StatSO stat in stats)
            {
                string enumText = textContainer.GetTextData(stat.statName).text[0];

                if (Enum.TryParse(enumText, out StatEnum statEnum))
                {
                    StatUI targetUI = _statUIContainer.GetStatUI(statEnum);

                    if (targetUI is null && excludeIndex < _excludeStatEnums.Count
                                         && statUIList.Any(ui =>
                                             ui.StatUIData.statEnum == _excludeStatEnums[excludeIndex]))
                    {
                        _statUIContainer.SetStatUI(_excludeStatEnums[excludeIndex], statEnum);
                        targetUI = _statUIContainer.GetStatUI(statEnum);
                        excludeIndex++;
                    }

                    SetStatUIElements(statCompo, stat, targetUI);
                }
            }
        }

        protected virtual void SetStatUIElements(EntityStatCompo statCompo, StatSO stat, StatUI targetUI)
        {
            StatSO targetStat = statCompo.GetStat(stat);
            targetUI?.SetStatText(targetStat.Value.ToString());
            targetUI?.SetEnable(true);
        }

        private void SetExcludeEnums(List<StatSO> stats)
        {
            _excludeStatEnums = statUiDataList.Select(uIData => uIData.statEnum)
                .Where(statEnum => stats.All(stat =>
                {
                    TextDataSO textData = textContainer.GetTextData(stat.statName);
                    return textData.text[0] != statEnum.ToString();
                }))
                .ToList();
        }

        public void SetTween(bool isActive)
        {
            Vector2 targetPos = isActive ? _originPos : hidePos;
            _activeTween?.Complete();
            _activeTween = _rectTrm.DOAnchorPos(targetPos, activeDuration);
        }
    }
}