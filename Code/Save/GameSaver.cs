using System;
using Code.EventSystems;
using Code.Managers;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Save
{
    [Serializable]
    public class GameSaveData
    {
        public int currentGold;
        public string userName;
        public float masterVolume;
        public float bgmVolume;
        public float sfxVolume;
    }
    
    public class GameSaver : MonoBehaviour, ISavable
    {
        [field:SerializeField] public SaveIdSO SaveID { get; private set; }

        public bool IsNameValid => !string.IsNullOrEmpty(_userName);
        
        [SerializeField] private GameEventChannelSO goldChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        
        [Inject] private GoldManager _goldManager;
        private string _userName;

        private void Awake()
        {
            uiChannel.AddListener<NameRegisterEvent>(HandleNameRegister);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<NameRegisterEvent>(HandleNameRegister);
        }

        private void HandleNameRegister(NameRegisterEvent evt)
        {
            _userName = evt.userName;
        }

        public string GetSaveData()
        {
            GameSaveData saveData = new GameSaveData
            {
                currentGold = _goldManager.CurrentGold,
                userName = _userName
            };

            return JsonUtility.ToJson(saveData);
        }

        public void RestoreData(string loadedData)
        {
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(loadedData);
            goldChannel.RaiseEvent(GoldEvents.GoldSetEvent.Initializer(saveData.currentGold));
            _userName = saveData.userName;
        }
    }
}