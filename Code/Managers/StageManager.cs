using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Core;
using Code.ETC;
using Code.EventSystems;
using Code.Save;
using Code.Scenes.Initializer;
using Code.Stages;
using HNLib.Dependencies;
using UnityEngine;
using Newtonsoft.Json;

namespace Code.Managers
{
    [Serializable]
    public class StageSaveData
    {
        public int lastClearStage;
    }

    public class FriendlyMatchData
    {
        public SpawnableSaveData spawnableSaveData;
        public int reward;
        public int costLimit;
    }

    [Provide]
    public class StageManager : MonoBehaviour, ISceneInit, ISavable, IDependencyProvider, ICombatStageBehavior,
        IOnceManager
    {
        public int Priority => 0;
        
        public StageDataSO CurrentStageData { get; private set; }
        public bool IsFirstClear { get; private set; }
        public int LastClearStage { get; private set; }
        public bool IsCombatStage { get; set; }

        [field: SerializeField] public SaveIdSO SaveID { get; private set; }

        [SerializeField] private GameEventChannelSO stageChannel;
        [SerializeField] private GameEventChannelSO sceneChannel;

        private List<IStageDataReceive> _dataReceives = new List<IStageDataReceive>();
        private FriendlyMatchData _matchData;
        private bool _isSceneChanged = true;

        public void Initialize()
        {
            stageChannel.AddListener<SetStageEvent>(HandleSetStage);
            stageChannel.AddListener<StageClearEvent>(HandleStageClear);
        }

        private void OnDestroy()
        {
            stageChannel.RemoveListener<SetStageEvent>(HandleSetStage);
            stageChannel.RemoveListener<StageClearEvent>(HandleStageClear);
        }

        private void HandleStageClear(StageClearEvent evt)
        {
            int clearedNum = evt.stageData.stageNum;

            if (LastClearStage < clearedNum)
                LastClearStage = clearedNum;
        }

        public void OnSceneInit()
        {
            _isSceneChanged = true;
            StartCoroutine(SetStageDatas());
        }

        public void OnSceneExit()
        {
        }

        private void HandleSetStage(SetStageEvent evt)
        {
            if (evt.isFriendlyMatch)
            {
                _matchData = evt.matchData;
                
                CurrentStageData = ScriptableObject.CreateInstance<StageDataSO>();
                CurrentStageData.stageNum = -1;
                CurrentStageData.costLimit = _matchData.costLimit;
                CurrentStageData.reward = _matchData.reward;
            }
            else
            {
                CurrentStageData = evt.stageData;
                IsFirstClear = evt.isFirstClear;

                _matchData = null;
            }
        }

        private IEnumerator SetStageDatas()
        {
            if (IsCombatStage == false || CurrentStageData is null) yield break;

            StageData targetStageData = null;

            if (_matchData != null)
            {
                targetStageData = new StageData()
                {
                    spawnables = _matchData.spawnableSaveData,
                    stageNum = -1
                };
            }
            else
            {
                TextAsset loadedJson = Resources.Load<TextAsset>("StageData");
                StageDatas stageDatas = JsonConvert.DeserializeObject<StageDatas>(loadedJson.text, JsonSetting.JsonSettings);
                targetStageData = stageDatas.stages.FirstOrDefault(data => data.stageNum == CurrentStageData.stageNum);
            }

            if (_isSceneChanged == false)
            {
                foreach (IStageDataReceive dataReceive in _dataReceives)
                    dataReceive.DataReceive(targetStageData);

                yield break;
            }

            _dataReceives = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IStageDataReceive>().ToList();

            foreach (IStageDataReceive dataReceive in _dataReceives)
                dataReceive.DataReceive(targetStageData);

            _isSceneChanged = false;
        }

        public string GetSaveData()
        {
            StageSaveData stageSaveData = new StageSaveData()
            {
                lastClearStage = LastClearStage
            };

            return JsonConvert.SerializeObject(stageSaveData);
        }

        public void RestoreData(string loadedData)
        {
            StageSaveData stageSaveData = JsonConvert.DeserializeObject<StageSaveData>(loadedData);
            if (stageSaveData.lastClearStage > LastClearStage)
                LastClearStage = stageSaveData.lastClearStage;
        }
    }
}