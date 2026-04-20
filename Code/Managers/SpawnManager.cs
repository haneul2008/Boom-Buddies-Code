using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Towers;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    public class SpawnManager<TSpawnable> : MonoBehaviour, ICombatStageBehavior, IOnceManager where TSpawnable : ISpawnable 
    {
        public int Priority => 0;
        public bool IsCombatStage { get; set; }
        
        [SerializeField] protected SpawnableListSO spawnableList;
        [SerializeField] protected GameEventChannelSO systemChannel;
        [SerializeField] protected GameEventChannelSO goldChannel;

        protected readonly Dictionary<SpawnableDataSO, MonoBehaviour> _spawnablePairs = new Dictionary<SpawnableDataSO, MonoBehaviour>();
        protected GoldManager _goldManager;
        protected ISpawnable _currentSpawnable;

        public virtual void Initialize()
        {
            foreach (SpawnableDataSO spawnableData in spawnableList.spawnableDataList)
            {
                TSpawnable spawnable = spawnableData.prefab.GetComponent<TSpawnable>();
                if (spawnable == null)
                {
                    Debug.LogWarning($"tower is null : {spawnableData.prefab.gameObject.name}");
                    continue;
                }
                _spawnablePairs.Add(spawnable.SpawnableData, spawnableData.prefab.GetComponent<TSpawnable>() as MonoBehaviour);
            }

            _goldManager = CreateOnceManager.Instance.GetManager<GoldManager>();
            
            systemChannel.AddListener<SpawnableSellEvent>(HandleSpawnableSell);
        }

        protected virtual void OnDestroy()
        {
            systemChannel.RemoveListener<SpawnableSellEvent>(HandleSpawnableSell);
        }

        protected virtual void HandleSpawnableSell(SpawnableSellEvent evt)
        {
            if(evt.spawnable is MonoBehaviour monoBehaviour)
                Destroy(monoBehaviour.gameObject);
            
            systemChannel.RaiseEvent(SystemEvents.BakeMapEvent);
        }

        public virtual T Create<T>(SpawnableDataSO data) where T : class
        {
            ISpawnable spawnable = _spawnablePairs.GetValueOrDefault(data) as ISpawnable;

            if (spawnable == null) return default;

            MonoBehaviour prefab = spawnable as MonoBehaviour;

            if (prefab == null) return default;
            
            SpawnableDataSO targetData = spawnableList.spawnableDataList
                .FirstOrDefault(data => data.prefab == prefab.gameObject);
            
            return Spawn(prefab, targetData) as T;
        }

        public virtual ISpawnable Create(SpawnableDataSO spawnableData)
        {
            MonoBehaviour prefab = _spawnablePairs.GetValueOrDefault(spawnableData);

            return Spawn(prefab, spawnableData);
        }
        
        protected virtual ISpawnable Spawn(MonoBehaviour prefab, SpawnableDataSO targetData)
        {
            ISpawnable item = Instantiate(prefab) as ISpawnable;

            if (item == null) return default;
            
            item.Spawn();
            _currentSpawnable = item;
            
            if(item is IPlaceable iPlaceable)
                systemChannel.RaiseEvent(SystemEvents.PlaceStartEvent.Initializer(iPlaceable));

            if (item is Entity entity)
                entity.OnPlaceEvent.AddListener(HandlePlace);
            
            return item;
        }

        protected virtual void HandlePlace(Entity entity)
        {
            entity.OnPlaceEvent.RemoveListener(HandlePlace);
            
            int requiredGold = _currentSpawnable.SpawnableData.requiredGold;
            
            GoldChangeEvent evt = GoldEvents.GoldChangeEvent;
            evt.gold = -requiredGold;
            evt.onChangeGold += HandleChangeGold;
            goldChannel.RaiseEvent(evt);
            
            systemChannel.RaiseEvent(SystemEvents.SpawnableBuyEvent.Initializer(_currentSpawnable.SpawnableData));
        }

        private void HandleChangeGold(bool isSuccess)
        {
            GoldEvents.GoldChangeEvent.onChangeGold -= HandleChangeGold;

            int requiredGold = _currentSpawnable.SpawnableData.requiredGold;
            
            if (isSuccess && _goldManager.CurrentGold >= requiredGold)
            {
                Create(_currentSpawnable.SpawnableData);
            }
        }
    }
}