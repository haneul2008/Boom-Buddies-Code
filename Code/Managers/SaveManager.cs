using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.ETC;
using Code.EventSystems;
using Code.Firebase;
using Code.Save;
using Firebase.Auth;
using HNLib.Dependencies;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Managers
{
    [Serializable]
    public class SaveData
    {
        public int saveId;
        public string data;
    }

    [Serializable]
    public struct DataCollection
    {
        public List<SaveData> dataList;
    }

    [Provide]
    public class SaveManager : MonoBehaviour, IDependencyProvider, IOnceManager
    {
        public event Action OnDataLoaded;
        public int Priority => 10;
        
        [SerializeField] private GameEventChannelSO saveChannel;
        [SerializeField] private string saveDataKey = "savedGame";

        private readonly List<SaveData> _unUsedData = new List<SaveData>();
        private FirebaseManager _firebaseManager;
        private string _path;

        public void Initialize()
        {
            saveChannel.AddListener<SaveEvent>(HandleSave);
            saveChannel.AddListener<LoadEvent>(HandleLoad);
            saveChannel.AddListener<SaveToDatabaseEvent>(HandleSaveToDatabase);

            _firebaseManager = new FirebaseManager();
            _firebaseManager.Initialize();

            _path = $"{Application.persistentDataPath}/SaveData.json";
        }

        private void OnDestroy()
        {
            saveChannel.RemoveListener<SaveEvent>(HandleSave);
            saveChannel.RemoveListener<LoadEvent>(HandleLoad);
            saveChannel.RemoveListener<SaveToDatabaseEvent>(HandleSaveToDatabase);
        }

        private void HandleSaveToDatabase(SaveToDatabaseEvent evt)
        {
            string dataJson = GetDataToSave();
            File.WriteAllText(_path, dataJson);
            _firebaseManager.SaveData(dataJson);
        }

        private void HandleSave(SaveEvent evt)
        {
            if (evt.isSaveToFile == false)
                SaveGameToPrefs();
            else
                SaveGameToFile();            
        }

        private void SaveGameToFile()
        {
            string dataJson = GetDataToSave();
            File.WriteAllText(_path, dataJson);
        }

        private void SaveGameToPrefs()
        {
            string dataJson = GetDataToSave();
            PlayerPrefs.SetString(saveDataKey, dataJson);
        }

        private string GetDataToSave()
        {
            IEnumerable<ISavable> savableObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();

            List<SaveData> saveDataList = new List<SaveData>();

            foreach (ISavable savable in savableObjects)
            {
                saveDataList.Add(new SaveData
                {
                    saveId = savable.SaveID.saveID,
                    data = savable.GetSaveData()
                });
            }

            saveDataList.AddRange(_unUsedData);

            DataCollection saveDataCollection = new DataCollection { dataList = saveDataList };

            return JsonConvert.SerializeObject(saveDataCollection, JsonSetting.JsonSettings);
        }

        private void HandleLoad(LoadEvent evt)
        {
            if (evt.isLoadFromFile == false)
                LoadFromPrefs();
            else
                LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(_path) == false) return;

            string json = File.ReadAllText(_path);
            DataCollection collection = JsonConvert.DeserializeObject<DataCollection>(json);
            RestoreData(collection);
        }

        private void LoadFromPrefs()
        {
            /*string loadedJson = PlayerPrefs.GetString(saveDataKey, string.Empty);
            RestoreData(loadedJson);*/
        }

        private void RestoreData(DataCollection collection)
        {
            IEnumerable<ISavable> savableObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavable>();

            _unUsedData.Clear();

            if (collection.dataList != null)
            {
                foreach (SaveData saveData in collection.dataList)
                {
                    ISavable target = savableObjects.FirstOrDefault(s => s.SaveID.saveID == saveData.saveId);

                    if (target != null)
                    {
                        target.RestoreData(saveData.data);
                    }
                    else
                    {
                        _unUsedData.Add(saveData);
                    }
                }
            }

            OnDataLoaded?.Invoke();
        }

        public void GetAllUserData(bool includeCurrentUser, Action<List<DataCollection>> onComplete = null)
        {
            _firebaseManager.GetAllUserData(includeCurrentUser, onComplete);
        }

        public bool IsDataEmpty() => !File.Exists(_path);
        
        [ContextMenu("Clear Save Data")]
        public void ClearSaveData()
        {
            _path = $"{Application.persistentDataPath}/SaveData.json";
            File.WriteAllText(_path, string.Empty);
        }

        [ContextMenu("Sign out")]
        public void SignOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
    }
}
