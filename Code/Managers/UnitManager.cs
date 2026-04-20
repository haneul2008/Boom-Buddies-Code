using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Extension;
using Code.Input;
using Code.Towers;
using Code.Units;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [Serializable]
    public struct InitUnit
    {
        public UnitDataSO unitData;
        public int cnt;
    }
    
    [Provide]
    public class UnitManager : SpawnManager<Unit>, IPlaceMediator, IDependencyProvider
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private GameEventChannelSO unitChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private List<InitUnit> initUnitList;
        [SerializeField] private float spawnDelay;
        [Inject] private TowerManager _towerManager;
        [Inject] private SaveManager _saveManager;

        private readonly Dictionary<Collider, Unit> _unitPairs = new Dictionary<Collider, Unit>();
        private Vector3Int _prevPos;
        private float _lastSpawnTime = -999f;
        private int _cost;

        public override void Initialize()
        {
            base.Initialize();

            _saveManager.OnDataLoaded += HandleDataLoaded;

            if (_saveManager.IsDataEmpty())
            {
                StartCoroutine(InitUnitCoroutine());
            }
        }

        private IEnumerator InitUnitCoroutine()
        {
            yield return null;
            
            foreach (InitUnit initUnit in initUnitList)
            {
                for (int i = 0; i < initUnit.cnt; ++i)
                {
                    systemChannel.RaiseEvent(SystemEvents.SpawnableBuyEvent.Initializer(initUnit.unitData));
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if(_saveManager == null) return;
            
            _saveManager.OnDataLoaded -= HandleDataLoaded;
        }

        private void HandleDataLoaded()
        {
            StageManager stageManager = CreateOnceManager.Instance.GetManager<StageManager>();
            
            if(stageManager.CurrentStageData == null) return;
            
            _cost = stageManager.CurrentStageData.costLimit;
            uiChannel.RaiseEvent(UIEvents.UseCostEvent.Initializer(_cost));
        }

        protected override ISpawnable Spawn(MonoBehaviour iSpawnable, SpawnableDataSO targetData)
        {
            if (IsCombatStage)
            {
                Unit unit = base.Spawn(iSpawnable, targetData) as Unit;
                if (unit is null) return default;
            
                unit.Initialize();
            
                return unit;
            }

            systemChannel.RaiseEvent(SystemEvents.SpawnableBuyEvent.Initializer(targetData));
            
            int requiredGold = targetData.requiredGold;
                
            GoldChangeEvent evt = GoldEvents.GoldChangeEvent;
            evt.gold = -requiredGold;
            goldChannel.RaiseEvent(evt);

            return null;
        }

        public bool TryPlace(Vector3Int pos, IPlaceable placeable)
        {
            if (_lastSpawnTime + spawnDelay > Time.time && _prevPos == pos || IsNotSpawnArea()) return false;
            
            UnitDataSO unitData = (UnitDataSO)_currentSpawnable.SpawnableData;

            if (_cost - unitData.cost < 0) return false;
            
            Vector3Int[,] unitBound = new Vector3Int[1, 1]
            {
                {
                    pos
                }
            };
            
            return _towerManager.CanPlace(unitBound);
        }

        private bool IsNotSpawnArea()
        {
            RaycastHit hit = playerInput.GroundHit;
            return IsCombatStage && hit.collider != null && !hit.collider.CompareTag("SpawnableGround");
        }

        protected override void HandlePlace(Entity entity)
        {
            entity.OnPlaceEvent.RemoveListener(HandlePlace);
            
            Unit unit = entity as Unit;
            
            Create(_currentSpawnable.SpawnableData);
            
            if(unit is null) return;
            
            _unitPairs.Add(unit.Collider, unit);

            _prevPos = unit.transform.position.ToVector3Int();

            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.AddListener(HandleUnitDead);
            
            UnitDataSO unitData = unit.SpawnableData as UnitDataSO;
            _cost -= unitData.cost;
            uiChannel.RaiseEvent(UIEvents.UseCostEvent.Initializer(_cost));
            
            _lastSpawnTime = Time.time;
        }

        private void HandleUnitDead(Entity entity)
        {
            EntityHealth health = entity.GetCompo<EntityHealth>();
            health.OnDead.RemoveListener(HandleUnitDead);

            if (entity is Unit unit)
            {
                _unitPairs.Remove(unit.Collider);
                
                if(_unitPairs.Count == 0)
                    unitChannel.RaiseEvent(UnitEvents.AllUnitDeadEvent);
            }
        }

        public Unit GetUnit(Collider targetCollider)
        {
            if (targetCollider == null) return null;
            return _unitPairs.GetValueOrDefault(targetCollider);
        }

        protected override void HandleSpawnableSell(SpawnableSellEvent evt)
        {
        }
    }
}