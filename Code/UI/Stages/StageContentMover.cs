using System;
using Code.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Code.UI.Stages
{
    public class StageContentMover : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private Transform moveTarget;
        [SerializeField] private float dragSpeed = 1000f;
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float minSize, maxSize;
        [SerializeField] private float limitX, limitY;

        private float _baseLimitX, _baseLimitY;
        private RectTransform _targetRectTrm;
        private Vector2 _lastMousePosition;
        private bool _isActive;

        private void Awake()
        {
            _targetRectTrm = moveTarget as RectTransform;
            playerInput.OnZoomPressed += HandleZoomPressed;

            _baseLimitX = limitX;
            _baseLimitY = limitY;
        }

        private void OnDestroy()
        {
            playerInput.OnZoomPressed -= HandleZoomPressed;
        }

        public void SetActive(bool isActive) => _isActive = isActive;

        private void HandleZoomPressed(float y)
        {
            if (_isActive == false) return;

            Vector3 originScale = moveTarget.localScale;
            moveTarget.localScale = originScale + Vector3.one * y * zoomSpeed;

            float clampedScale = Mathf.Clamp(moveTarget.localScale.x, minSize, maxSize);
            moveTarget.localScale = Vector3.one * clampedScale;

            float scaleFactor = clampedScale / minSize;
            limitX = _baseLimitX * scaleFactor;
            limitY = _baseLimitY * scaleFactor;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (_isActive == false) return;

            _targetRectTrm.anchoredPosition -= playerInput.MovementKey * moveSpeed;

            float x = Mathf.Clamp(_targetRectTrm.anchoredPosition.x, -limitX, limitX);
            float y = Mathf.Clamp(_targetRectTrm.anchoredPosition.y, -limitY, limitY);

            _targetRectTrm.anchoredPosition = new Vector2(x, y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastMousePosition = Mouse.current.position.value;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 currentPos = Mouse.current.position.value;
            Vector2 delta = (currentPos - _lastMousePosition).normalized;

            _targetRectTrm.anchoredPosition += delta * dragSpeed * Time.unscaledDeltaTime;

            _lastMousePosition = currentPos;
        }
    }
}