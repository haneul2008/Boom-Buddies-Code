using System;
using Code.EventSystems;
using Code.Managers;
using Code.Scenes.Initializer;
using DG.Tweening;
using HNLib.Dependencies;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Transition
{
    public class TransitionUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO sceneChannel;
        [SerializeField] private float fadeInValue, fadeOutValue;
        [SerializeField] private float duration;
        [SerializeField] private float fadeInDelay;

        private Image _image;
        private Material _mat;
        private readonly int _valueHash = Shader.PropertyToID("_Value");

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.material = new Material(_image.material);
            _mat = _image.material;

            DOVirtual.DelayedCall(fadeInDelay, () =>
            {
                HandlePopUpUI(UIEvents.PopUpTransitionUI.Initializer(true, null));
            });

            uiChannel.AddListener<PopUpTransitionUI>(HandlePopUpUI);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<PopUpTransitionUI>(HandlePopUpUI);
        }

        private void HandlePopUpUI(PopUpTransitionUI evt)
        {
            float targetValue = evt.isFadeIn ? fadeInValue : fadeOutValue;

            _mat.SetFloat(_valueHash, fadeInValue - targetValue);
            
            gameObject.SetActive(true);
            
            _mat.DOFloat(targetValue, _valueHash, duration).SetUpdate(true).OnComplete(() =>
            {
                if (evt.isFadeIn)
                    gameObject.SetActive(false);
                else
                    sceneChannel.RaiseEvent(SceneEvents.SceneChangeEvent.Initializer(evt.nextScene));
            });
        }
    }
}