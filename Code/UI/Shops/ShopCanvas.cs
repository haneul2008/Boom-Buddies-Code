using System;
using System.Collections.Generic;
using System.Linq;
using Code.EventSystems;
using Code.Input;
using Code.Shops;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.UI.Shops
{
    [Serializable]
    public struct ActivableElement
    {
        public RectTransform rectTrm;
        public Vector2 hidePos;
        public float duration;
    }
    
    public class ShopCanvas : ShopElement
    {
        [SerializeField] private List<ActivableElement> activableList = new List<ActivableElement>();
        [SerializeField] private PlayerInputSO playerInput;

        private readonly Dictionary<ActivableElement, Vector2> _originPosPairs = new Dictionary<ActivableElement, Vector2>();
        private readonly Dictionary<ActivableElement, Tween> _activeTweenPairs = new Dictionary<ActivableElement, Tween>();

        protected override void Awake()
        {
            base.Awake();

            activableList.ForEach(element =>
            {
                _originPosPairs.Add(element, element.rectTrm.anchoredPosition);
                _activeTweenPairs.Add(element, null);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _activeTweenPairs.Values.ToList().ForEach(tween => tween?.Kill());
        }

        public override void SetActive(bool isActive)
        {
            foreach (ActivableElement element in activableList)
            {
                Vector2 targetPos = isActive ? _originPosPairs[element] : element.hidePos;
                _activeTweenPairs[element]?.Complete();
                _activeTweenPairs[element] = element.rectTrm.DOAnchorPos(targetPos, element.duration).SetUpdate(true);
            }

            Time.timeScale = isActive ? 0f : 1f;
            playerInput.SetEnable(!isActive);
        }
    }
}