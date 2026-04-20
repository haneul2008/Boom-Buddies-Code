using Code.EventSystems;
using Code.Extension;
using Code.Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Cameras
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO cameraChannel;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private float minFOV, maxFOV;
        [SerializeField] private float zoomAmount;
        [SerializeField] private float moveSpeed;

        private CinemachineCamera _cam;
        private bool _isCamMode = true;

        private void Awake()
        {
            playerInput.OnZoomPressed += HandleZoomPressed;
            cameraChannel.AddListener<CameraModeChangeEvent>(HandleCameraModeChange);
            _cam = GetComponent<CinemachineCamera>();
        }

        private void OnDestroy()
        {
            playerInput.OnZoomPressed -= HandleZoomPressed;
            cameraChannel.RemoveListener<CameraModeChangeEvent>(HandleCameraModeChange);
        }

        private void HandleCameraModeChange(CameraModeChangeEvent evt)
        {
            _isCamMode = evt.isCamMode;
        }

        private void Update()
        {
            if(_isCamMode == false || playerInput.MovementKey.IsEquals(Vector2.zero)) return;

            Vector3 camRotation = Camera.main.transform.eulerAngles;
            
            Vector3 moveDir = new Vector3(playerInput.MovementKey.x, 0, playerInput.MovementKey.y);
            Vector3 movementKey = Quaternion.Euler(0, camRotation.y, 0) * moveDir;
            transform.position += movementKey * (Time.deltaTime * moveSpeed);
        }

        private void HandleZoomPressed(float yMovement)
        {
            if(Time.timeScale == 0) return;
            
            float res = _cam.Lens.FieldOfView - yMovement * zoomAmount;
            _cam.Lens.FieldOfView = Mathf.Clamp(res, minFOV, maxFOV);
        }
    }
}