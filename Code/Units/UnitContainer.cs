using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Units
{
    [Provide]
    public class UnitContainer : MonoBehaviour, ISpawnableSavable, IDependencyProvider
    {
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO unitChannel;
        [SerializeField] private SpawnableListSO unitList;

        private readonly Dictionary<UnitDataSO, int> _unitPairs = new Dictionary<UnitDataSO, int>();

        private void Awake()
        {
            systemChannel.AddListener<SpawnableBuyEvent>(HandleSpawnableBuy);
            systemChannel.AddListener<SpawnableSellEvent>(HandleSpawnableSell);
            unitChannel.AddListener<UnitSpawnEvent>(HandleUnitSpawn);
            unitChannel.AddListener<CheckLastUnitSpawnEvent>(HandleCheckLastUnitSpawn);
        }

        private void OnDestroy()
        {
            systemChannel.RemoveListener<SpawnableBuyEvent>(HandleSpawnableBuy);
            systemChannel.RemoveListener<SpawnableSellEvent>(HandleSpawnableSell);
            unitChannel.RemoveListener<UnitSpawnEvent>(HandleUnitSpawn);
            unitChannel.RemoveListener<CheckLastUnitSpawnEvent>(HandleCheckLastUnitSpawn);
        }

        private void HandleSpawnableSell(SpawnableSellEvent evt)
        {
            if (evt.spawnable is Unit unit)
            {
                UnitDataSO unitData = unit.SpawnableData as UnitDataSO;
                
                if(unitData is null) return;

                if (_unitPairs.ContainsKey(unitData) && _unitPairs[unitData] > 0)
                {
                    _unitPairs[unitData]--;
                    
                    if (_unitPairs[unitData] == 0)
                        _unitPairs.Remove(unitData);
                }
            }
        }

        private void HandleCheckLastUnitSpawn(CheckLastUnitSpawnEvent evt)
        {
            foreach (var pair in _unitPairs)
            {
                if(pair.Value > 0)
                    return;
            }
            
            unitChannel.RaiseEvent(UnitEvents.LastUnitSpawnEvent);
        }

        private void HandleUnitSpawn(UnitSpawnEvent evt)
        {
            if (_unitPairs.ContainsKey(evt.unitData))
            {
                _unitPairs[evt.unitData]--;
            }
        }

        private void HandleSpawnableBuy(SpawnableBuyEvent evt)
        {
            UnitDataSO unitData = evt.spawnableData as UnitDataSO;
            
            if(unitData is null) return;
            
            if (_unitPairs.TryAdd(unitData, 1) == false)
            {
                _unitPairs[unitData]++;
            }
            
            uiChannel.RaiseEvent(UIEvents.UnitAddEvent.Initializer(unitData));
        }

        public SpawnableInfoDatas GetSaveData()
        {
            SpawnableInfoDatas infoDatas = new SpawnableInfoDatas
            {
                saveDatas = GetSaveDatas(),
                spawnableType = SpawnableType.Unit
            };
            return infoDatas;
        }

        private List<SpawnableInfoData> GetSaveDatas()
        {
            List<SpawnableInfoData> result = new List<SpawnableInfoData>();

            foreach (var unitKeyValuePair in _unitPairs)
            {
                SpawnableInfoData infoData = new SpawnableInfoData
                {
                    spawnableName = unitKeyValuePair.Key.spawnableName,
                    amount = unitKeyValuePair.Value
                };

                result.Add(infoData);
            }

            return result;
        }

        public void RestoreData(List<SpawnableInfoDatas> saveDatas)
        {
            SpawnableInfoDatas targetDatas = saveDatas.FirstOrDefault(data =>
                data.spawnableType == SpawnableType.Unit);

            if (targetDatas == null) return;

            foreach (SpawnableInfoData data in targetDatas.saveDatas)
            {
                UnitDataSO targetData = unitList.spawnableDataList
                    .FirstOrDefault(unitData => unitData.spawnableName == data.spawnableName) as UnitDataSO;

                if (targetData is null) continue;

                _unitPairs.Add(targetData, data.amount);
                
                for(int i = 0; i < data.amount; ++i)
                    uiChannel.RaiseEvent(UIEvents.UnitAddEvent.Initializer(targetData));
            }
        }
    }
}