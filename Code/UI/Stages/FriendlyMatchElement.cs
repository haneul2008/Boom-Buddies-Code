using Code.Core;
using Code.EventSystems;
using Code.Managers;
using Code.Scenes;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.UI.Stages
{
    public class FriendlyMatchElement : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO goldChannel;
        [SerializeField] private GameEventChannelSO stageChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private SceneDataSO combatScene;
        [SerializeField] private int expenseGold;
        [SerializeField] private int costLimit;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI combatScoreText;
        [SerializeField] private TextMeshProUGUI winGoldText;
        [SerializeField] private TextMeshProUGUI expenseText;
        [SerializeField] private float colorTweenDuration;

        private Tween _colorTween;
        private SpawnableSaveData _saveData;
        private int _reward;
        private bool _isStarted;

        public void Initialize(string userName, int combatScore, int reward, SpawnableSaveData saveData)
        {
            nameText.text = userName;
            combatScoreText.text = $"전투 점수 : {combatScore}";
            winGoldText.text = reward.ToString();
            
            _saveData = saveData;
            _reward = reward;
        }

        public void EnterCombatStage()
        {
            if(_isStarted) return;
            
            GoldChangeEvent goldEvt = GoldEvents.GoldChangeEvent;
            goldEvt.onChangeGold += HandleChangeGold;
            goldChannel.RaiseEvent(GoldEvents.GoldChangeEvent.Initializer(-expenseGold));
        }

        private void HandleChangeGold(bool isSuccess)
        {
            GoldEvents.GoldChangeEvent.onChangeGold -= HandleChangeGold;

            if (isSuccess)
            {
                _isStarted = true;
                
                FriendlyMatchData matchData = new FriendlyMatchData()
                {
                    spawnableSaveData = _saveData,
                    reward = _reward,
                    costLimit = costLimit
                };
                
                SetStageEvent evt = StageEvents.SetStageEvent.Initializer(null, false, true, matchData);
                stageChannel.RaiseEvent(evt);
                
                uiChannel.RaiseEvent(UIEvents.PopUpTransitionUI.Initializer(false, combatScene));
            }
            else
            {
                _colorTween?.Complete();
                _colorTween = expenseText.DOColor(Color.red, colorTweenDuration).SetUpdate(true)
                    .SetLoops(2, LoopType.Yoyo);
            }
        }
    }
}