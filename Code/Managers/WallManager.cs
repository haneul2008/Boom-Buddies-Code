using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.ETC;
using Code.EventSystems;
using Code.Extension;
using Code.Levels;
using Code.Scenes.Initializer;
using Code.Stages;
using Code.Upgrade;
using Code.Walls;
using HNLib.Dependencies;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Managers
{
    [Provide]
    public class WallManager : SpawnManager<Wall>, IPlaceMediator, IDependencyProvider, ISpawnableSavable, IStageDataReceive, ISceneInit
    {
        [SerializeField] private int mapWidth, mapHeight, combatPosAdder;
        [SerializeField] private GameEventChannelSO wallChannel;

        [Inject] private TowerManager _towerManager;
        [Inject] private SaveManager _saveManager;
        private readonly Dictionary<Vector3Int, Wall> _wallPairs = new Dictionary<Vector3Int, Wall>();
        private AstarCalculator _astarCalculator;
        
        public void OnSceneInit()
        {
            _saveManager.OnDataLoaded += HandleDataLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(_saveManager == null) return;
            
            _saveManager.OnDataLoaded -= HandleDataLoaded;
        }

        private void HandleDataLoaded()
        {
            if(IsCombatStage == false) return;

            Dictionary<Vector2Int, AstarCalculator.NodeType> nodePairs =
                new Dictionary<Vector2Int, AstarCalculator.NodeType>();

            for (int x = 0; x <= mapWidth * 2; x += 2)
            {
                for (int y = 0; y <= mapHeight * 2; y += 2)
                {
                    AstarCalculator.NodeType nodeType = AstarCalculator.NodeType.Empty;

                    Vector3Int pos = new Vector3Int(x, 0, y) + Vector3Int.one;
                    pos.y = 0;
                    
                    if (_towerManager.IsTowerBound(pos)) nodeType = AstarCalculator.NodeType.Tower;
                    else if (IsWall(pos)) nodeType = AstarCalculator.NodeType.Wall;

                    nodePairs.Add(new Vector2Int(x / 2, y / 2), nodeType);
                }
            }

            _astarCalculator = new AstarCalculator(mapWidth, mapHeight, nodePairs);
        }

        public Wall GetOptimalWall(Vector3Int pos, Vector3Int targetPos)
        {
            Vector2Int startPos = new Vector2Int(pos.x, pos.z) / 2;
            Vector2Int endPos = new Vector2Int(targetPos.x, targetPos.z) / 2;
            Vector2Int? result = _astarCalculator.GetOptimalWall(startPos, endPos);

            if (result is null) return null;

            Vector3Int wallPos = new Vector3Int(result.Value.x, 0, result.Value.y);
            return _wallPairs.GetValueOrDefault(wallPos);
        }

        public bool TryPlace(Vector3Int pos, IPlaceable placeable)
        {
            Vector3Int[,] bound = new Vector3Int[1, 1]
            {
                {
                    pos
                }
            };
            return IsWall(pos) == false && _towerManager.CanPlace(bound);
        }

        public bool IsWall(Vector3Int pos) => _wallPairs.ContainsKey(pos);

        protected override ISpawnable Spawn(MonoBehaviour iSpawnable, SpawnableDataSO targetData)
        {
            ISpawnable item = base.Spawn(iSpawnable, targetData);
            Wall wall = item as Wall;

            if (wall == null) return default;
            
            wall.Initialize(IsCombatStage);

            return item;
        }

        protected override void HandlePlace(Entity entity)
        {
            base.HandlePlace(entity);

            Wall wall = entity as Wall;

            if (wall is null) return;

            _wallPairs.Add(entity.transform.position.ToVector3Int(), wall);

            wallChannel.RaiseEvent(WallEvents.CheckNearWallEvent.Initializer(_wallPairs));

            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.AddListener(HandleWallDead);
        }

        private void HandleWallDead(Entity entity)
        {
            _wallPairs.Remove(entity.transform.position.ToVector3Int());

            Vector3Int pos = entity.transform.position.ToVector3Int();
            
            _astarCalculator.SetNodeType(pos.x / 2, pos.z / 2, AstarCalculator.NodeType.Empty);
            
            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.RemoveListener(HandleWallDead);
        }

        #region Save

        public SpawnableInfoDatas GetSaveData()
        {
            SpawnableInfoDatas infoDatas = new SpawnableInfoDatas
            {
                saveDatas = GetInfoDatas(),
                spawnableType = SpawnableType.Wall
            };

            return infoDatas;
        }

        private List<SpawnableInfoData> GetInfoDatas()
        {
            List<SpawnableInfoData> result = new List<SpawnableInfoData>();

            List<Wall> wallTypes = _wallPairs.Values
                .GroupBy(wall => wall.GetType())
                .Select(group => group.First())
                .ToList();

            foreach (Wall wall in wallTypes)
            {
                SpawnableInfoData info = new SpawnableInfoData
                {
                    spawnableName = wall.SpawnableData.spawnableName,
                    instanceDatas = new List<PolymorphicWrapper>()
                };
                
                result.Add(info);
            }

            foreach (Wall spawnedWall in _wallPairs.Values)
            {
                SpawnableInfoData targetInfo = result
                    .FirstOrDefault(info => info.spawnableName == spawnedWall.SpawnableData.spawnableName);

                SpawnableInstanceData instanceData = new SpawnableInstanceData()
                {
                    pos = spawnedWall.transform.position,
                    upgrade = spawnedWall.UpgradeLevel,
                    requiredGold = spawnedWall.SpawnableData.requiredGold,
                    eulerAngle = spawnedWall.transform.eulerAngles
                };

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
            if(IsCombatStage) return;
            
            SpawnWallsFromData(saveDatas);
        }
        
        #endregion
        
        public void DataReceive(StageData stageDatas)
        {
            if(IsCombatStage == false) return;

            SpawnWallsFromData(stageDatas.spawnables.infoDatas);
        }
        
        private void SpawnWallsFromData(List<SpawnableInfoDatas> saveDatas)
        {
            SpawnableInfoDatas targetInfos = saveDatas
                .FirstOrDefault(infos => infos.spawnableType == SpawnableType.Wall);

            if (targetInfos == null) return;
            
            _wallPairs.Clear();

            foreach (SpawnableInfoData info in targetInfos.saveDatas)
            {
                WallDataSO targetData = spawnableList.spawnableDataList
                    .FirstOrDefault(data => data.spawnableName == info.spawnableName) as WallDataSO;

                if (targetData is null) continue;

                MonoBehaviour prefab = _spawnablePairs.Values.FirstOrDefault(spawnable =>
                    spawnable.gameObject == targetData.prefab);

                foreach (PolymorphicWrapper wrapper in info.instanceDatas)
                {
                    Type type = Type.GetType(wrapper.type);
                    SpawnableInstanceData instanceData = JsonConvert.DeserializeObject(wrapper.json, type, JsonSetting.JsonSettings)
                        as SpawnableInstanceData;
                    
                    Vector3 pos = instanceData.pos;

                    if (IsCombatStage)
                        pos += Vector3.one.RemoveY() * combatPosAdder;

                    Wall wall = Instantiate(prefab, pos, Quaternion.Euler(instanceData.eulerAngle)) as Wall;

                    if (wall is null) continue;

                    wall.Spawn();
                    wall.OnPlaceEvent?.Invoke(wall);
                    wall.CompletePlace(pos);
                    wall.Initialize(IsCombatStage);
                    _wallPairs.Add(pos.ToVector3Int(), wall);

                    EntityHealth health = wall.GetCompo<EntityHealth>();
                    health.OnDead.AddListener(HandleWallDead);
                    
                    while (wall.UpgradeLevel != instanceData.upgrade)
                    {
                        UpgradeDataSO upgradeData = wall.MyUpgrade.GetUpgrade(wall.UpgradeLevel);
                        wall.Upgrade(upgradeData);
                        wall.UpgradeLevel++;
                    }
                }
            }

            wallChannel.RaiseEvent(WallEvents.CheckNearWallEvent.Initializer(_wallPairs));
        }

        protected override void HandleSpawnableSell(SpawnableSellEvent evt)
        {
            if (evt.spawnable is Wall wall)
            {
                _wallPairs.Remove(wall.transform.position.ToVector3Int());
                
                EntityHealth health = wall.GetCompo<EntityHealth>();
                health.OnDead.RemoveListener(HandleWallDead);
                
                wallChannel.RaiseEvent(WallEvents.CheckNearWallEvent.Initializer(_wallPairs));
                
                base.HandleSpawnableSell(evt);
            }
        }

        public void OnSceneExit()
        {
        }

        [SerializeField] private int upgrade;
        [ContextMenu("Wall upgrade")]
        public void UpgradeWall()
        {
            foreach (Wall wall in _wallPairs.Values)
            {
                for (int i = 0; i < upgrade; ++i)
                {
                    wall.Upgrade(wall.MyUpgrade.GetUpgrade(wall.UpgradeLevel));
                    wall.UpgradeLevel++;
                }
            }
        }
    }
}