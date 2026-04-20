using System;
using Code.EventSystems;
using Code.Scenes;
using UnityEngine;

namespace Code.UI.Combat
{
    public class SurrenderCheckUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private SceneDataSO baseScene;

        private bool _isActive;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Active()
        {
            _isActive = !_isActive;
            gameObject.SetActive(_isActive);
        }
        
        public void Surrender()
        {
            uiChannel.RaiseEvent(UIEvents.PopUpTransitionUI.Initializer(false, baseScene));
        }
    }
}