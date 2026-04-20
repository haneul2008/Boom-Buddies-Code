using System;
using System.Collections.Generic;
using Code.EventSystems;
using Code.Scenes;
using Code.Stages;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.UI.Stages
{
    public class StageEnterUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private Transform starTrm;
        [SerializeField] private GameObject starPrefab;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Vector2 hidePos;
        [SerializeField] private float activeDuration;
        [SerializeField] private SceneDataSO combatSceneData;

        private readonly List<GameObject> _starList = new List<GameObject>();
        private RectTransform _rectTrm;
        private Vector2 _originPos;
        private Tween _activeTween;

        private void Awake()
        {
            _rectTrm = transform as RectTransform;
            _originPos = _rectTrm.anchoredPosition;
            _rectTrm.anchoredPosition = hidePos;
            uiChannel.AddListener<PopUpStageEnterUI>(HandlePopUpUI);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<PopUpStageEnterUI>(HandlePopUpUI);
        }

        private void HandlePopUpUI(PopUpStageEnterUI evt)
        {
            SetData(evt);
            Active(evt.isActive);
        }

        private void SetData(PopUpStageEnterUI evt)
        {
            StageDataSO stageData = evt.stageData;
            
            if(stageData is null) return;
            
            stageText.text = $"Stage{stageData.stageNum}";

            int difficult = stageData.difficult;
            
            if (difficult > _starList.Count)
            {
                int addAmount = difficult - _starList.Count;
                
                for (int i = 0; i < addAmount; ++i)
                {
                    GameObject newStar = Instantiate(starPrefab, starTrm);
                    _starList.Add(newStar);
                }
            }

            for (int i = 0; i < _starList.Count; ++i)
            {
                _starList[i].SetActive(i < difficult);
            }
            
            rewardText.text = evt.isFirstClear ? "최초 클리어 보상" : "클리어 보상";
            goldText.text = evt.isFirstClear ? stageData.firstClearReward.ToString() : stageData.reward.ToString();
        }

        public void Active(bool isActive)
        {
            Vector2 targetPos = isActive ? _originPos : hidePos;

            _activeTween?.Complete();
            _activeTween = _rectTrm.DOAnchorPos(targetPos, activeDuration).SetUpdate(true);
        }

        public void EnterCombatStage()
        {
            uiChannel.RaiseEvent(UIEvents.PopUpTransitionUI.Initializer(false, combatSceneData));
        }
    }
}