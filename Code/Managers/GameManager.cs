using System;
using Code.Core;
using Code.Scenes.Initializer;
using HNLib.Dependencies;
using UnityEngine;
using Unity.Cinemachine;

namespace Code.Managers
{
    [Provide]
    public class GameManager : MonoBehaviour, IOnceManager, IDependencyProvider, ISceneInit
    {
        public int Priority => 0;

        public bool IsGameStarted { get; set; }
        
        [SerializeField] private SelectDataSO selectData;

        private bool _isInit;
        
        public void OnSceneInit()
        {
            selectData.Initialize();

            CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
            
            _isInit = true;
        }

        public void OnSceneExit()
        {
        }
        
        public void Initialize()
        {
            
        }

        private void OnDestroy()
        {
            if(_isInit == false) return;
            
            selectData.Disable();
        }
    }
}