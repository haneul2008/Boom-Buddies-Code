using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.ETC;
using Code.EventSystems;
using Code.Extension;
using Code.Save;
using Code.Stages;
using Code.Towers;
using Code.Towers.Impl;
using Code.UI.Combat;
using Code.Upgrade;
using HNLib.Dependencies;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Managers
{
    [Provide]
    public class TowerManager : SpawnManager<Tower>, IPlaceMediator, IDependencyProvider, ISpawnableSavable,
        IStageDataReceive
    {
        public List<Tower> SpawnedTowerList { get; private set; } = new List<Tower>();

        [SerializeField] private List<TowerDataSO> excludeDataList;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private int combatPosAdder;
        [Inject] private WallManager _wallManager;
        [Inject] private PlaceManager _placeManager;
        [Inject] private SaveManager _saveManager;

        private readonly Dictionary<StarCondition, bool> _starConditionPairs = new Dictionary<StarCondition, bool>();
        private CoreTower _coreTower;
        private int _towerTotal;

        private void Start()
        {
            if (_saveManager.IsDataEmpty())
                AddCoreTower();
        }

        private void AddCoreTower()
        {
            _coreTower = FindAnyObjectByType<CoreTower>();

            if (SpawnedTowerList.Contains(_coreTower)) return;

            _coreTower.Initialize((TowerDataSO)_coreTower.SpawnableData, IsCombatStage);
            _coreTower.Spawn();
            _coreTower.CompletePlace(_coreTower.transform.position);

            SpawnedTowerList.Add(_coreTower);

            EntityHealth health = _coreTower.GetCompo<EntityHealth>();
            health.OnDead.AddListener(HandleTowerDead);
        }

        private void HandleTowerDead(Entity entity)
        {
            SpawnedTowerList.Remove((Tower)entity);

            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.RemoveListener(HandleTowerDead);

            int currentTowerCnt = SpawnedTowerList.Count;
            float damagePercent = currentTowerCnt / (float)_towerTotal;

            ActiveCore();

            TryRaiseConditionEvent(damagePercent <= 0.5f, StarCondition.Damaged50Percent);
            TryRaiseConditionEvent(damagePercent <= 0.3f, StarCondition.Damaged70Percent);
            TryRaiseConditionEvent(entity.GetType() == typeof(CoreTower), StarCondition.CoreDestroy);
        }

        private void TryRaiseConditionEvent(bool value, StarCondition condition)
        {
            if (value && _starConditionPairs[condition] == false)
            {
                uiChannel.RaiseEvent(UIEvents.StarConditionSendEvent.Initializer(condition));
                _starConditionPairs[condition] = true;
            }
        }

        public bool TryPlace(Vector3Int pos, IPlaceable placeable)
        {
            Tower tower = placeable as Tower;

            if (tower is null || CanPlace(tower.CalculateBound(pos)) == false) return false;

            return true;
        }

        public bool CanPlace(Vector3Int[,] bound)
        {
            HashSet<Vector3Int> positions = new HashSet<Vector3Int>();

            foreach (Vector3Int pos in bound)
            {
                if (_wallManager.IsWall(pos)) return false;
                positions.Add(pos);
            }

            foreach (Tower tower in SpawnedTowerList)
            {
                foreach (Vector3Int boundPos in tower.Bound)
                {
                    if (positions.Contains(boundPos))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        protected override ISpawnable Spawn(MonoBehaviour iSpawnable, SpawnableDataSO targetData)
        {
            ISpawnable item = base.Spawn(iSpawnable, targetData);

            Tower tower = item as Tower;
            if (tower is null) return default;

            tower.Initialize((TowerDataSO)targetData, IsCombatStage);

            return tower;
        }

        protected override void HandlePlace(Entity entity)
        {
            base.HandlePlace(entity);

            Tower tower = entity as Tower;

            if (tower is null) return;

            SpawnedTowerList.Add(tower);

            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.AddListener(HandleTowerDead);
        }

        public bool IsTowerBound(Vector3Int pos)
        {
            foreach (Tower tower in SpawnedTowerList)
            {
                HashSet<Vector3Int> boundSet = new HashSet<Vector3Int>();
                foreach (Vector3Int bound in tower.Bound)
                    boundSet.Add(bound);

                if (boundSet.Contains(pos)) return true;
            }

            return false;
        }

        #region Save

        public SpawnableInfoDatas GetSaveData()
        {
            SpawnableInfoDatas infoDatas = new SpawnableInfoDatas
            {
                saveDatas = GetInfoDatas(),
                spawnableType = SpawnableType.Tower
            };

            return infoDatas;
        }

        private List<SpawnableInfoData> GetInfoDatas()
        {
            List<SpawnableInfoData> result = new List<SpawnableInfoData>();

            List<Tower> towerTypes = SpawnedTowerList
                .GroupBy(tower => tower.GetType())
                .Select(group => group.First())
                .Where(tower => excludeDataList.Contains(tower.SpawnableData) == false)
                .ToList();

            foreach (Tower tower in towerTypes)
            {
                SpawnableInfoData info = new SpawnableInfoData
                {
                    spawnableName = tower.SpawnableData.spawnableName,
                    instanceDatas = new List<PolymorphicWrapper>()
                };
                result.Add(info);
            }

            foreach (Tower spawnedTower in SpawnedTowerList)
            {
                SpawnableInfoData targetInfo = result
                    .FirstOrDefault(info => info.spawnableName == spawnedTower.SpawnableData.spawnableName);

                SpawnableInstanceData instanceData;

                if (spawnedTower is ISpawnableInstanceOverride instanceOverride)
                {
                    instanceData = instanceOverride.GetInstanceData();
                }
                else
                {
                    instanceData = new SpawnableInstanceData()
                    {
                        pos = spawnedTower.transform.position,
                        upgrade = spawnedTower.UpgradeLevel,
                        requiredGold = spawnedTower.SpawnableData.requiredGold,
                        eulerAngle = spawnedTower.transform.eulerAngles
                    };
                }

                PolymorphicWrapper wrapper = new PolymorphicWrapper
                {
                    type = instanceData.GetType().AssemblyQualifiedName,
                    json = JsonConvert.SerializeObject(instanceData, JsonSetting.JsonSettings)
                };

                targetInfo?.instanceDatas.Add(wrapper);
            }

            foreach (SpawnableInfoData info in result)
                info.amount = info.instanceDatas.Count;

            return result;
        }

        public void RestoreData(List<SpawnableInfoDatas> saveDatas)
        {
            if (IsCombatStage) return;

            _starConditionPairs.Clear();

            SpawnTowersFromData(saveDatas);
            AddCoreTower();
        }

        #endregion

        public void DataReceive(StageData stageDatas)
        {
            if (IsCombatStage == false) return;

            InitStarCondition();

            SpawnTowersFromData(stageDatas.spawnables.infoDatas);
            AddCoreTower();
            ActiveCore();

            _towerTotal = SpawnedTowerList.Count;
        }

        private void InitStarCondition()
        {
            foreach (StarCondition condition in Enum.GetValues(typeof(StarCondition)))
            {
                _starConditionPairs.Add(condition, false);
            }
        }

        private void SpawnTowersFromData(List<SpawnableInfoDatas> saveDatas)
        {
            SpawnableInfoDatas targetInfos = saveDatas
                .FirstOrDefault(infos => infos.spawnableType == SpawnableType.Tower);

            if (targetInfos == null) return;

            SpawnedTowerList.Clear();

            foreach (SpawnableInfoData info in targetInfos.saveDatas)
            {
                TowerDataSO targetData = spawnableList.spawnableDataList
                    .FirstOrDefault(data => data.spawnableName == info.spawnableName) as TowerDataSO;

                if (targetData is null || excludeDataList.Contains(targetData)) continue;

                MonoBehaviour prefab = _spawnablePairs.Values.FirstOrDefault(spawnable =>
                    spawnable.gameObject == targetData.prefab);

                foreach (PolymorphicWrapper wrapper in info.instanceDatas)
                {
                    Type type = Type.GetType(wrapper.type);
                    SpawnableInstanceData instanceData =
                        JsonConvert.DeserializeObject(wrapper.json, type, JsonSetting.JsonSettings)
                            as SpawnableInstanceData;

                    Vector3 pos = instanceData.pos;
                    if (IsCombatStage)
                        pos += Vector3.one.RemoveY() * combatPosAdder;
                    
                    Tower tower =
                        Instantiate(prefab, pos, Quaternion.Euler(instanceData.eulerAngle)) as Tower;

                    if (tower is null) continue;

                    tower.Spawn();
                    tower.Initialize(targetData, IsCombatStage);
                    tower.OnPlaceEvent?.Invoke(tower);
                    tower.CompletePlace(instanceData.pos);
                    SpawnedTowerList.Add(tower);

                    EntityHealth health = tower.GetCompo<EntityHealth>();
                    health.OnDead.AddListener(HandleTowerDead);

                    while (tower.UpgradeLevel != instanceData.upgrade)
                    {
                        UpgradeDataSO upgradeData = tower.MyUpgrade.GetUpgrade(tower.UpgradeLevel);
                        tower.Upgrade(upgradeData);
                        tower.UpgradeLevel++;
                    }

                    if (tower is ISpawnableInstanceOverride instanceOverride)
                        instanceOverride.SetData(instanceData);
                }
            }
        }

        protected override void HandleSpawnableSell(SpawnableSellEvent evt)
        {
            if (evt.spawnable is Tower tower)
            {
                SpawnedTowerList.Remove(tower);

                EntityHealth health = tower.GetCompo<EntityHealth>();
                health.OnDead.RemoveListener(HandleTowerDead);

                systemChannel.RaiseEvent(SystemEvents.BakeMapEvent);

                base.HandleSpawnableSell(evt);
            }
        }
        
        private void ActiveCore()
        {
            if (SpawnedTowerList.Count == 1 && SpawnedTowerList[0].GetType() == typeof(CoreTower))
                _coreTower.ActiveHealth();
        }
    }
}