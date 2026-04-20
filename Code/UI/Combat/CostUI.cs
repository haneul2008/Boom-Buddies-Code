using System;
using Code.EventSystems;
using Code.Managers;
using Code.Scenes.Initializer;
using HNLib.Dependencies;
using TMPro;
using UnityEngine;

namespace Code.UI.Combat
{
    public class CostUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TextMeshProUGUI costText;

        private void Awake()
        {
            uiChannel.AddListener<UseCostEvent>(HandleUseCost);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<UseCostEvent>(HandleUseCost);
        }

        private void HandleUseCost(UseCostEvent evt)
        {
            costText.text = evt.cost.ToString();
        }

        public void OnSceneExit()
        {
        }
    }
}