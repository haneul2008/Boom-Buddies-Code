using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.EventSystems;
using Code.Units;
using DG.Tweening;
using UnityEngine;

namespace Code.UI.Units
{
    public class UnitContainerUI : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private UnitUI unitUI;
        [SerializeField] private Transform contentTrm;
        [SerializeField] private Vector2 hidePos;
        [SerializeField] private float activeDuration;

        private RectTransform _rectTrm;
        private Vector2 _originPos;
        private bool _isActive;
        private Tween _activeTween;
        private readonly Dictionary<UnitDataSO, UnitUI> _unitUIPairs = new Dictionary<UnitDataSO, UnitUI>();

        private void Awake()
        {
            _rectTrm = transform as RectTransform;
            _originPos = _rectTrm.anchoredPosition;
            _rectTrm.anchoredPosition = hidePos;
            
            uiChannel.AddListener<UnitAddEvent>(HandleUnitAdd);
            systemChannel.AddListener<SpawnableSellEvent>(HandleSpawnableSell);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<UnitAddEvent>(HandleUnitAdd);
            systemChannel.RemoveListener<SpawnableSellEvent>(HandleSpawnableSell);
            _activeTween?.Kill();
        }

        private void HandleSpawnableSell(SpawnableSellEvent evt)
        {
            UnitDataSO unitData = evt.spawnable.SpawnableData as UnitDataSO;
            
            if(unitData is null) return;

            if (_unitUIPairs.TryGetValue(unitData, out UnitUI ui))
            {
                ui.DecreaseAmount();
                if (ui.Amount == 0)
                {
                    Destroy(_unitUIPairs[unitData].gameObject);
                    _unitUIPairs.Remove(unitData);
                }
            }
        }

        private void HandleUnitAdd(UnitAddEvent evt)
        {
            UnitDataSO unitData = evt.unitData;

            if (_unitUIPairs.TryGetValue(unitData, out UnitUI ui))
            {
                ui.AddAmount();
            }
            else
            {
                UnitUI spawnedUI = Instantiate(unitUI, contentTrm);
                spawnedUI.Initialize(unitData);
                _unitUIPairs.Add(unitData, spawnedUI);
            }
        }

        public void ActiveToggle()
        {
            if(_activeTween != null) return;
            
            _isActive = !_isActive;

            Vector2 targetPos = _isActive ? _originPos : hidePos;
            _activeTween = _rectTrm.DOAnchorPos(targetPos, activeDuration).SetUpdate(true)
                .OnComplete(() => _activeTween = null);
        }
    }
}