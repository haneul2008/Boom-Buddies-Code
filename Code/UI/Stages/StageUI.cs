using System;
using Code.EventSystems;
using Code.Stages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Stages
{
    public class StageUI : MonoBehaviour
    {
        [field:SerializeField] public int StageNum { get; private set; }
        [SerializeField] private GameEventChannelSO stageChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image lockImage;
        [SerializeField] private Sprite clearBackground;
        [SerializeField] private TextMeshProUGUI stageText;

        private bool _isFirstClear;
        private bool _isActiveStage;
        private StageDataSO _stageData;
        
        private void OnValidate()
        {
            if(StageNum == 0 || stageText == null) return;
            
            gameObject.name = $"StageUI_{StageNum}";
            stageText.text = StageNum.ToString();
        }

        public void Initialize(StageDataSO stageData, bool isFirstClear)
        {
            _stageData = stageData;
            lockImage.enabled = true;
            _isFirstClear = isFirstClear;
            
            if(isFirstClear == false)
                backgroundImage.sprite = clearBackground;
        }

        public void ActiveStage()
        {
            _isActiveStage = true;
            lockImage.enabled = false;
        }

        public void SetStage()
        {
            if(_isActiveStage == false) return;
            
            stageChannel.RaiseEvent(StageEvents.SetStageEvent.Initializer(_stageData, _isFirstClear, false, null));
            uiChannel.RaiseEvent(UIEvents.PopUpStageEnterUI.Initializer(_stageData, _isFirstClear, true));
        }
    }
}