using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.EventSystems;
using Code.Input;
using Code.Scenes;
using Code.Scenes.Initializer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Managers
{
    public class SceneManager : MonoBehaviour, IOnceManager
    {
        public int Priority => 0;
        
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private GameEventChannelSO sceneChannel;
        [SerializeField] private GameEventChannelSO saveChannel;
        [SerializeField] private SceneDataSO initScene;

        private SceneDataSO _currentSceneData;
        
        public void Initialize()
        {
            sceneChannel.AddListener<SceneChangeEvent>(HandleSceneChange);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleSceneLoaded;
            _currentSceneData = initScene;
        }

        private void OnDestroy()
        {
            sceneChannel.RemoveListener<SceneChangeEvent>(HandleSceneChange);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnApplicationQuit()
        {
            if (_currentSceneData.isSaveData)
            {
                saveChannel.RaiseEvent(SaveEvents.SaveToDatabaseEvent);
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            StartCoroutine(LoadSceneCoroutine(1));
        }

        private IEnumerator LoadSceneCoroutine(int frame)
        {
            for (int i = 0; i < frame; ++i)
            {
                yield return null;
            }

            if (_currentSceneData == null) yield break;

            Time.timeScale = 1;
            playerInput.SetEnable(true);

            SceneInit();
            saveChannel.RaiseEvent(SaveEvents.LoadEvent.Initializer(true));
        }

        private void SceneInit()
        {
            List<ISceneInit> sceneInits = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ISceneInit>().OrderByDescending(init => init.Priority).ToList();
            
            sceneInits.ForEach(sceneInit => sceneInit.OnSceneInit());
        }

        private void HandleSceneChange(SceneChangeEvent evt)
        {
            SceneDataSO nextSceneData = evt.sceneData;
            if (_currentSceneData.isSaveData)
            {
                saveChannel.RaiseEvent(SaveEvents.SaveEvent.Initializer(true));
            }

            _currentSceneData = nextSceneData;
            
            List<ISceneInit> sceneInits = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ISceneInit>().ToList();
            
            sceneInits.ForEach(sceneInit => sceneInit.OnSceneExit());
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneData.sceneName);
        }
    }
}