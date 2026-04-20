using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.EventSystems;
using Code.Managers;
using Code.Scenes;
using Code.Stages;
using Code.UI.Units;
using Code.Units;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Combat
{
    public enum StarCondition
    {
        Damaged50Percent,
        Damaged70Percent,
        CoreDestroy
    }
    
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;
        
        [Header("Channel Setting")]
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private GameEventChannelSO unitChannel;
        [SerializeField] private GameEventChannelSO stageChannel;
        [SerializeField] private GameEventChannelSO goldChannel;
        
        [Header("Unit Setting")]
        [SerializeField] private RectTransform usedUnitTrm;
        [SerializeField] private UnitUI usedUnitUI;
        
        [Header("Active Setting")]
        [SerializeField] private float activeDelay;
        [SerializeField] private Vector2 hidePos;
        [SerializeField] private float activeDuration;
        
        [Header("Star Setting")]
        [SerializeField] private float starActiveDelay;
        [SerializeField] private float starActiveTerm;
        
        [Header("Scene Setting")] 
        [SerializeField] private SceneDataSO baseScene;

        private readonly Dictionary<UnitDataSO, UnitUI> _unitPairs = new Dictionary<UnitDataSO, UnitUI>();
        private  List<StarCondition> _activeConditions = new List<StarCondition>();
        private List<StarUI> _starUIList = new List<StarUI>();
        private RectTransform _rectTrm;
        private Vector2 _originPos;
        private Tween _activeTween;
        private bool _isLastUnitSpawned;
        private WaitForSecondsRealtime _starActiveTerm;

        private void Awake()
        {
            uiChannel.AddListener<PopUpResultUIEvent>(HandlePopUpResultUI);
            systemChannel.AddListener<PlaceCompleteEvent>(HandlePlaceComplete);
            unitChannel.AddListener<LastUnitSpawnEvent>(HandleLastUnitSpawn);
            unitChannel.AddListener<AllUnitDeadEvent>(HandleAllUnitDead);
            uiChannel.AddListener<StarConditionSendEvent>(HandleStarConditionSend);

            _rectTrm = transform as RectTransform;

            _originPos = _rectTrm.anchoredPosition;
            _rectTrm.anchoredPosition = hidePos;

            _starUIList = GetComponentsInChildren<StarUI>().ToList();

            _starActiveTerm = new WaitForSecondsRealtime(starActiveTerm);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<PopUpResultUIEvent>(HandlePopUpResultUI);
            systemChannel.RemoveListener<PlaceCompleteEvent>(HandlePlaceComplete);
            unitChannel.RemoveListener<LastUnitSpawnEvent>(HandleLastUnitSpawn);
            unitChannel.RemoveListener<AllUnitDeadEvent>(HandleAllUnitDead);
            uiChannel.RemoveListener<StarConditionSendEvent>(HandleStarConditionSend);
        }

        private void HandleStarConditionSend(StarConditionSendEvent evt)
        {
            _activeConditions.Add(evt.condition);
        }

        private void HandleAllUnitDead(AllUnitDeadEvent evt)
        {
            if(_isLastUnitSpawned == false) return;

            HandlePopUpResultUI(UIEvents.PopUpResultUIEvent.Initializer(false));
        }

        private void HandleLastUnitSpawn(LastUnitSpawnEvent evt) => _isLastUnitSpawned = true;

        private void HandlePlaceComplete(PlaceCompleteEvent evt)
        {
            ISpawnable spawnable = evt.placeable as ISpawnable;

            if (spawnable?.SpawnableData is not UnitDataSO unitData) return;

            if (_unitPairs.TryGetValue(unitData, out UnitUI unitUI))
            {
                unitUI.AddAmount();
            }
            else
            {
                UnitUI spawnedUI = Instantiate(usedUnitUI, usedUnitTrm);
                spawnedUI.Initialize(unitData);
                _unitPairs.Add(unitData, spawnedUI);
                
                SetContentTrmSize(_unitPairs.Count);
            }
        }

        private void HandlePopUpResultUI(PopUpResultUIEvent evt)
        {
            resultText.text = evt.isClear ? "승리" : "패배";

            if (evt.isClear)
            {
                StageManager stageManager = CreateOnceManager.Instance.GetManager<StageManager>();
                StageDataSO currentStageData = stageManager.CurrentStageData;
                
                stageChannel.RaiseEvent(StageEvents.StageClearEvent.Initializer(currentStageData));

                int reward = stageManager.IsFirstClear ? currentStageData.firstClearReward : currentStageData.reward;
                goldChannel.RaiseEvent(GoldEvents.SetAddGoldEvent.Initializer(reward));
            }
            
            DOVirtual.DelayedCall(activeDelay, () => SetActive(true));
        }

        private void SetActive(bool isActive)
        {
            _activeTween?.Complete();

            Vector2 targetPos = isActive ? _originPos : hidePos;
            float timeScale = isActive ? 0f : 1f;
            Time.timeScale = timeScale;

            _activeTween = _rectTrm.DOAnchorPos(targetPos, activeDuration).SetUpdate(true)
                .OnComplete(() => HandleActiveComplete(isActive));
        }

        private void HandleActiveComplete(bool isActive)
        {
            if(isActive == false) return;
            
            DOVirtual.DelayedCall(starActiveDelay, () => StartCoroutine(SpawnStars()));
        }

        private IEnumerator SpawnStars()
        {
            foreach (StarUI starUI in _starUIList)
            {
                if(_activeConditions.Contains(starUI.Condition) == false) continue;
                
                starUI.SetStar();
                yield return _starActiveTerm;
            }
        }

        private void SetContentTrmSize(int elementCnt)
        {
            HorizontalLayoutGroup layoutGroup = usedUnitTrm.GetComponent<HorizontalLayoutGroup>();
            RectTransform elementTrm = usedUnitUI.transform as RectTransform;

            float paddings = layoutGroup.padding.left + layoutGroup.padding.right;
            float spacings = (elementCnt - 1) * layoutGroup.spacing;
            float x = elementTrm.sizeDelta.x * elementCnt + paddings + spacings;
            usedUnitTrm.sizeDelta = new Vector2(x, usedUnitTrm.sizeDelta.y);
        }

        public void BackToBase()
        {
            uiChannel.RaiseEvent(UIEvents.PopUpTransitionUI.Initializer(false, baseScene));
        }
    }
}