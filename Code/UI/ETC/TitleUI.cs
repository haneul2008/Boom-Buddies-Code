using System;
using Code.EventSystems;
using Code.Managers;
using Code.Scenes;
using DG.Tweening;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.UI.ETC
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private float disableDelay;

        [Inject] private GameManager _gameManager;

        private void Awake()
        {
            if(_gameManager.IsGameStarted)
                gameObject.SetActive(false);
            else
            {
                _gameManager.IsGameStarted = true;
                DOVirtual.DelayedCall(disableDelay, () =>
                {
                    gameObject.SetActive(false);
                    uiChannel.RaiseEvent(UIEvents.PopUpTransitionUI.Initializer(true, null));
                });
            }
        }
    }
}