using System;
using Code.EventSystems;
using Code.Input;
using Code.Managers;
using Code.Save;
using Code.Scenes.Initializer;
using HNLib.Dependencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.ETC
{
    public class NameRegisterUI : MonoBehaviour, ISceneInit
    {
        public int Priority => -10;
        
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private GameEventChannelSO saveChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private int nameLimit;

        [Inject] private SaveManager _saveManager;
        
        public void OnSceneInit()
        {
            if (_saveManager.IsDataEmpty())
            {
                playerInput.SetEnable(false);
            }

            _saveManager.OnDataLoaded += HandleDataLoad;
        }

        private void OnDestroy()
        {
            _saveManager.OnDataLoaded -= HandleDataLoad;
        }

        private void HandleDataLoad()
        {
            GameSaver gameSaver = FindAnyObjectByType<GameSaver>();

            if (gameSaver.IsNameValid == false)
            {
                playerInput.SetEnable(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnValueChange()
        {
            if(inputField.text.Length > nameLimit)
                inputField.text = inputField.text.Substring(0, nameLimit);
        }
        
        public void OnRegister()
        {
            if(string.IsNullOrEmpty(inputField.text)) return;
            
            string userName = inputField.text;
            
            saveChannel.RaiseEvent(SaveEvents.SaveToDatabaseEvent);
            uiChannel.RaiseEvent(UIEvents.NameRegisterEvent.Initializer(userName));
            playerInput.SetEnable(true);
            gameObject.SetActive(false);
        }

        public void OnSceneExit()
        {
        }
    }
}