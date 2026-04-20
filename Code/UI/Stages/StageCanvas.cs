using System;
using Code.EventSystems;
using Code.UI.ETC;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.UI.Stages
{
    public class StageCanvas : GameCanvas
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private StageContentMover contentMover;
        [SerializeField] private RectTransform rootTrm;
        [SerializeField] private Vector2 hidePos;
        [SerializeField] private float activeDuration;
        
        private Vector2 _originPos;
        private Tween _activeTween;
        private bool _isActive;

        private void Awake()
        {
            uiChannel.AddListener<ToggleStageChoiceUI>(HandlePopUpUI);

            _originPos = rootTrm.anchoredPosition;
            rootTrm.anchoredPosition = hidePos;
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<ToggleStageChoiceUI>(HandlePopUpUI);
        }
        
        private void HandlePopUpUI(ToggleStageChoiceUI evt)
        {
            SetActive(!_isActive);
        }
        
        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            contentMover.SetActive(_isActive);
            
            if(_isActive == false)
                uiChannel.RaiseEvent(UIEvents.PopUpStageEnterUI.Initializer(null, false, false));

            _activeTween?.Complete();

            Vector2 targetPos = isActive ? _originPos : hidePos;
            float timeScale = isActive ? 0f : 1f;
            Time.timeScale = timeScale;

            _activeTween = rootTrm.DOAnchorPos(targetPos, activeDuration).SetUpdate(true);
        }
    }
}