using Code.EventSystems;
using Code.Input;
using Code.Managers;
using Code.References.Text;
using Code.Save;
using Code.Scenes.Initializer;
using DG.Tweening;
using HNLib.Dependencies;
using TMPro;
using UnityEngine;

namespace Code.UI.Tutorial
{
    public class TutorialUI : MonoBehaviour, ISceneInit
    {
        public int Priority => -20;

        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private TextDataSO tutorialTextData;
        [SerializeField] private TextContainerSO textContainer;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Vector2 hidePos;
        [SerializeField] private float activeDuration;
        [SerializeField] private float talkTerm;

        private RectTransform _rectTrm;
        private Vector2 _originPos;
        private bool _isTutorial;
        private float _lastTalkTime;
        private int _currentIndex;
        
        public void OnSceneInit()
        {
            GameSaver saver = FindAnyObjectByType<GameSaver>();
            _isTutorial = !saver.IsNameValid;
            uiChannel.AddListener<NameRegisterEvent>(HandleNameRegister);
            
            _rectTrm = transform as RectTransform;

            _originPos = _rectTrm.anchoredPosition;
            _rectTrm.anchoredPosition = hidePos;
        }

        public void OnSceneExit()
        {
            uiChannel.RemoveListener<NameRegisterEvent>(HandleNameRegister);
        }

        private void HandleNameRegister(NameRegisterEvent evt)
        {
            if (_isTutorial)
            {
                Time.timeScale = 0f;
                
                object key = tutorialTextData.GetKey();
                TextDataSO textData = textContainer.GetTextData(key);
                descText.text = textData.text[0];
                
                _rectTrm.DOAnchorPos(_originPos, activeDuration).SetUpdate(true)
                    .OnComplete(HandleActive);
            }
        }

        private void HandleActive()
        {
            _lastTalkTime = Time.time;
            playerInput.OnMouseLeftPressed += HandleLeftClick;
        }

        private void HandleLeftClick(bool isStart)
        {
            if(_lastTalkTime + talkTerm > Time.unscaledTime || _isTutorial == false) return;

            object key = tutorialTextData.GetKey();
            TextDataSO textData = textContainer.GetTextData(key);

            if (_currentIndex >= textData.text.Count - 1)
            {
                _rectTrm.DOAnchorPos(hidePos, activeDuration).SetUpdate(true).OnComplete(() => Time.timeScale = 1f);
                playerInput.OnMouseLeftPressed -= HandleLeftClick;
                _isTutorial = false;
                return;
            }
                
            descText.text = textData.text[++_currentIndex];
            _lastTalkTime = Time.unscaledTime;
        }
    }
}