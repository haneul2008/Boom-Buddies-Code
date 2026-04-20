using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.EventSystems;
using Code.Input;
using Code.Managers;
using Code.Scenes.Initializer;
using Code.Stages;
using DG.Tweening;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.UI.Stages
{
    public class StageChoiceUI : MonoBehaviour, ISceneInit
    {
        public int Priority => -15;
        
        [SerializeField] private StageListSO stageList;

        [Inject] private SaveManager _saveManager;
        [Inject] private StageManager _stageManager;

        private List<StageUI> _stageUiList;
        
        public void OnSceneInit()
        {
            if(_saveManager.IsDataEmpty())
                HandleDataLoaded();
            
            _saveManager.OnDataLoaded += HandleDataLoaded;
        }

        private void OnDestroy()
        {
            _saveManager.OnDataLoaded -= HandleDataLoaded;
        }

        private void HandleDataLoaded()
        {
            _stageUiList = GetComponentsInChildren<StageUI>()
                .OrderBy(ui => ui.StageNum).ToList();

            int lastClearStage = _stageManager.LastClearStage;

            for (int i = 0; i < stageList.stageDataList.Count; ++i)
            {
                StageDataSO stageData = stageList.stageDataList[i];

                bool isFirstClear = stageData.stageNum > lastClearStage;
                
                _stageUiList[i].Initialize(stageData, isFirstClear);
            }

            for (int i = 0; i < lastClearStage + 1; ++i)
            {
                if(i >= _stageUiList.Count) return;
                
                _stageUiList[i].ActiveStage();
            }
        }

        public void OnSceneExit()
        {
        }
    }
}