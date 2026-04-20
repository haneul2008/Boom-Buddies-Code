using System.Collections;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Save;
using Code.Stats;
using Code.Upgrade;
using UnityEngine;

namespace Code.Towers.Impl
{
    public class GoldTower : Tower, ISpawnableInstanceOverride
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO goldChannel;
        [SerializeField] private StatSO cooldownStat;
        [SerializeField] private StatSO goldStorageStat;
        [SerializeField] private StatSO goldProductionStat;
        [SerializeField] private Transform goldCanvasTrm;
        [SerializeField] private float extractLimit;

        public int Storage { get; private set; }
        public int MaxStorage { get; private set; }

        private float _cooldown;
        private int _goldProductionAmount;
        private float _lastProductionTime;
        private bool _isCombatStage;

        public override void Initialize(TowerDataSO towerData, bool isCombatStage)
        {
            base.Initialize(towerData, isCombatStage);

            _isCombatStage = isCombatStage;
            goldCanvasTrm.gameObject.SetActive(false);

            uiChannel.AddListener<ExtractGoldEvent>(HandleExtractGold);

            InitStat();
        }

        private void InitStat()
        {
            EntityStatCompo entityStatCompo = GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(cooldownStat);
            stat.OnValueChanged += HandleCooldownChanged;

            _cooldown = stat.BaseValue;

            stat = entityStatCompo.GetStat(goldStorageStat);
            stat.OnValueChanged += HandleStorageChanged;

            MaxStorage = (int)stat.BaseValue;

            stat = entityStatCompo.GetStat(goldProductionStat);
            stat.OnValueChanged += HandleGoldProductionChanged;

            _goldProductionAmount = (int)stat.BaseValue;
        }

        private void OnDestroy()
        {
            EntityStatCompo entityStatCompo = GetCompo<EntityStatCompo>(true);
            StatSO stat = entityStatCompo.GetStat(cooldownStat);
            stat.OnValueChanged -= HandleCooldownChanged;

            stat = entityStatCompo.GetStat(goldStorageStat);
            stat.OnValueChanged -= HandleStorageChanged;

            stat = entityStatCompo.GetStat(goldProductionStat);
            stat.OnValueChanged -= HandleGoldProductionChanged;

            uiChannel.RemoveListener<ExtractGoldEvent>(HandleExtractGold);
        }

        protected override void Update()
        {
            if (IsPlaced == false) return;

            AddGold();
        }

        private void AddGold()
        {
            if (_lastProductionTime + _cooldown > Time.unscaledTime) return;

            Storage = Mathf.Clamp(Storage + _goldProductionAmount, 0, MaxStorage);
            _lastProductionTime = Time.unscaledTime;
        }

        private void HandleExtractGold(ExtractGoldEvent evt)
        {
            if(Storage < Mathf.RoundToInt(MaxStorage / extractLimit)) return;
            
            CleanStorage();
            StartCoroutine(ExtractBtnActiveCoroutine());
        }

        public void CleanStorage()
        {
            if (Storage == 0) return;

            goldChannel.RaiseEvent(GoldEvents.GoldChangeEvent.Initializer(Storage));
            Storage = 0;
        }

        public override void CompletePlace(Vector3 pos)
        {
            base.CompletePlace(pos);

            if (_isCombatStage == false)
            {
                StartCoroutine(ExtractBtnActiveCoroutine());
                goldCanvasTrm.rotation = Camera.main.transform.rotation;
            }
        }

        public void ExtractGold() => uiChannel.RaiseEvent(UIEvents.ExtractGoldEvent);

        private IEnumerator ExtractBtnActiveCoroutine()
        {
            goldCanvasTrm.gameObject.SetActive(false);

            yield return new WaitUntil(() => Storage >= Mathf.RoundToInt(MaxStorage / extractLimit));

            goldCanvasTrm.gameObject.SetActive(true);
        }

        private void HandleGoldProductionChanged(StatSO stat, float prev, float current)
        {
            _goldProductionAmount = (int)current;
        }

        private void HandleStorageChanged(StatSO stat, float prev, float current)
        {
            MaxStorage = (int)current;
        }

        private void HandleCooldownChanged(StatSO stat, float prev, float current)
        {
            _cooldown = current;
        }

        public override void Spawn()
        {
        }

        public override void Upgrade(UpgradeDataSO upgradeData)
        {
            base.Upgrade(upgradeData);
            
            GoldTowerUpgradeDataSO goldTowerUpgradeData = upgradeData as GoldTowerUpgradeDataSO;
            
            if(goldTowerUpgradeData is null) return;
            
            meshFilter.transform.localPosition = goldTowerUpgradeData.visualPos;
        }

        public override void HandleDead()
        {
            IsDead = true;
            _collider.enabled = false;
            IsAutoRotate = false;
            Destroy(gameObject);
        }

        public void SetData(SpawnableInstanceData instanceData)
        {
            GoldTowerData goldTowerData = instanceData as GoldTowerData;
            
            if(goldTowerData is null) return;

            Storage = goldTowerData.currentStorage;
        }

        public SpawnableInstanceData GetInstanceData()
        {
            return new GoldTowerData()
            {
                pos = transform.position,
                eulerAngle = transform.eulerAngles,
                upgrade = UpgradeLevel,
                requiredGold = SpawnableData.requiredGold,
                currentStorage = Storage
            };
        }
    }
}