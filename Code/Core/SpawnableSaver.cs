using System;
using System.Collections.Generic;
using System.Linq;
using Code.ETC;
using Code.Save;
using HNLib.Dependencies;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Core
{
    public enum SpawnableType
    {
        Tower,
        Unit,
        Wall
    }

    [Serializable]
    public class SpawnableSaveData
    {
        public List<SpawnableInfoDatas> infoDatas;
    }

    [Serializable]
    public class SpawnableInfoDatas
    {
        public List<SpawnableInfoData> saveDatas;
        public SpawnableType spawnableType;
    }
    
    [Serializable]
    public class SpawnableInfoData
    {
        public string spawnableName;
        public int amount; // instanceDatas.Count
        public List<PolymorphicWrapper> instanceDatas;
    }

    [Serializable]
    public class SpawnableInstanceData
    {
        public Vector3 pos;
        public Vector3 eulerAngle;
        public int requiredGold;
        public int upgrade;
    }
    
    [Serializable]
    public class PolymorphicWrapper
    {
        public string type;
        public string json;
    }
    
    [Serializable]
    public class GoldTowerData : SpawnableInstanceData
    {
        public int currentStorage;
    }

    [Provide]
    public class SpawnableSaver : MonoBehaviour, ISavable, IDependencyProvider
    {
        [field: SerializeField] public SaveIdSO SaveID { get; private set; }
        
        public string GetSaveData()
        {
            List<SpawnableInfoDatas> infoDatas = GetSavables().Select(savable => savable.GetSaveData()).ToList();
            SpawnableSaveData saveData = new SpawnableSaveData()
            {
                infoDatas = infoDatas
            };

            string json = JsonConvert.SerializeObject(saveData, JsonSetting.JsonSettings);
            return json;
        }

        public void RestoreData(string loadedData)
        {
            SpawnableSaveData saveData = JsonConvert.DeserializeObject<SpawnableSaveData>(loadedData, JsonSetting.JsonSettings);
            List<SpawnableInfoDatas> infoDatas = saveData.infoDatas;
            List<ISpawnableSavable> savables = GetSavables().ToList();

            foreach (ISpawnableSavable savable in savables)
            {
                savable.RestoreData(infoDatas);
            }
        }
        
        private IEnumerable<ISpawnableSavable> GetSavables()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ISpawnableSavable>();
        }
    }
}