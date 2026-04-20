using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Core;
using Code.ETC;
using Code.Save;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Stages
{
    [Serializable]
    public class StageData
    {
        public int stageNum;
        public SpawnableSaveData spawnables;
    }

    [Serializable]
    public class StageDatas
    {
        public List<StageData> stages;
    }

    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private int stageNum;

        private string _path;
        private StageDatas _stageDatas;
        
        private void Awake()
        {
            _path = $"{Application.dataPath}/StageData.json";

            if (File.Exists(_path) == false)
            {
                _stageDatas = new StageDatas
                {
                    stages = new List<StageData>()
                };
                return;
            }

            string loadedJson = File.ReadAllText(_path);
            _stageDatas = JsonConvert.DeserializeObject<StageDatas>(loadedJson, JsonSetting.JsonSettings);
        }

        [ContextMenu("Generate Stage")]
        public void GenerateStage()
        {
            SpawnableSaveData saveData = new SpawnableSaveData
            {
                infoDatas = GetInfoDatas()
            };

            StageData stageData = new StageData
            {
                spawnables = saveData,
                stageNum = stageNum
            };

            StageData prevStageData = _stageDatas.stages.FirstOrDefault(stage => stage.stageNum == stageNum);

            if (prevStageData != null)
                _stageDatas.stages.Remove(prevStageData);

            _stageDatas.stages.Add(stageData);

            string json = JsonConvert.SerializeObject(_stageDatas, JsonSetting.JsonSettings);
            File.WriteAllText(_path, json);
        }

        private List<SpawnableInfoDatas> GetInfoDatas()
        {
            IEnumerable<ISpawnableSavable> savables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ISpawnableSavable>();
            return savables.Select(savable => savable.GetSaveData()).ToList();
        }
    }
}
