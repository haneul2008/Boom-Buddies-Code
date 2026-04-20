using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Input
{
    [CreateAssetMenu(menuName = "SO/PlayerInput")]
    public class PlayerInputSO : ScriptableObject, Control.IPlayerActions
    {
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private LayerMask whatIsTarget;
        
        public event Action OnCancelKeyPressed;
        public event Action OnRotateKeyPressed;
        public event Action<bool> OnMouseLeftPressed;
        public event Action<float> OnZoomPressed;

        public Vector2 MovementKey { get; private set; }
        public bool GroundHitValid { get; private set; }
        public bool TargetHitValid { get; private set; }
        public RaycastHit GroundHit => _cachedGroundHit;
        public RaycastHit TargetHit => _cachedTargetHit;

        private RaycastHit _cachedGroundHit;
        private RaycastHit _cachedTargetHit;
        private Control _controls;
        private Vector2 _screenPosition;
        private Vector3 _worldPosition;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Control();
                _controls.Player.SetCallbacks(this);
            }

            SetEnable(true);
        }

        private void OnDisable()
        {
            SetEnable(false);
        }

        public void SetEnable(bool isEnable)
        {
            if(isEnable)
                _controls.Player.Enable();
            else
                _controls.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementKey = context.ReadValue<Vector2>();
        }

        public void OnMouseLeft(InputAction.CallbackContext context)
        {
            if (context.started)
                OnMouseLeftPressed?.Invoke(true);
            else if(context.canceled)
                OnMouseLeftPressed?.Invoke(false);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            float y = context.ReadValue<Vector2>().y;
            OnZoomPressed?.Invoke(y);
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnRotateKeyPressed?.Invoke();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCancelKeyPressed?.Invoke();
        }

        public Vector3 GetWorldPosition()
        {
            UnityEngine.Camera mainCam = UnityEngine.Camera.main;
            Debug.Assert(mainCam != null, "No main camera in this scene.");
            Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, mainCam.farClipPlane, whatIsGround))
            {
                _worldPosition = hit.point;
            }

            return _worldPosition;
        }

        public void OnMousePos(InputAction.CallbackContext context)
        {
            _screenPosition = context.ReadValue<Vector2>();
            UpdateHitInfo();
        }

        private void UpdateHitInfo()
        {
            UnityEngine.Camera mainCam = UnityEngine.Camera.main;
            Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);

            GroundHitValid = Physics.Raycast(cameraRay, out _cachedGroundHit, mainCam.farClipPlane, whatIsGround);
            if (GroundHitValid)
            {
                _worldPosition = GroundHit.point;
            }

            TargetHitValid = Physics.Raycast(cameraRay, out _cachedTargetHit, mainCam.farClipPlane, whatIsTarget);
        }

        #region UnUsed

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}