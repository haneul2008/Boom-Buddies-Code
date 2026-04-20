using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Extension;
using Code.Input;
using DG.Tweening;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Managers
{
    [Provide]
    public class PlaceManager : MonoBehaviour, IDependencyProvider, IOnceManager
    {
        public int Priority => 0;
        
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private float rotateDuration;

        public bool IsPlanting { get; private set; }

        private IPlaceable _currentPlaceable;
        private GameObject _placeableObject;
        private Vector3Int _placePos;
        private Tween _rotateTween;
        private Vector3 _offset;
        private bool _isPlaceKeyHold;
        private Quaternion _prevTargetRotation;

        public void Initialize()
        {
            systemChannel.AddListener<PlaceStartEvent>(HandlePlaceStart);
            uiChannel.AddListener<ShopToggleEvent>(HandleShopToggle);

            playerInput.OnMouseLeftPressed += HandlePlaceKeyPressed;
            playerInput.OnRotateKeyPressed += HandleRotateKeyPressed;
            playerInput.OnCancelKeyPressed += CancelPlace;
        }

        private void OnDestroy()
        {
            systemChannel.RemoveListener<PlaceStartEvent>(HandlePlaceStart);
            uiChannel.RemoveListener<ShopToggleEvent>(HandleShopToggle);
            
            playerInput.OnMouseLeftPressed -= HandlePlaceKeyPressed;
            playerInput.OnRotateKeyPressed -= HandleRotateKeyPressed;
            playerInput.OnCancelKeyPressed -= CancelPlace;
        }

        private void HandleShopToggle(ShopToggleEvent evt)
        {
            if (evt.isActive)
            {
                CancelPlace();
            }
        }

        public void CancelPlace()
        {
            IsPlanting = false;
            
            if (_currentPlaceable != null)
            {
                Destroy(_placeableObject);
                ResetPlaceable();
            }
        }

        private void HandlePlaceStart(PlaceStartEvent evt)
        {
            IsPlanting = true;

            PlaceObject(_placePos);

            _currentPlaceable = evt.placeable;
            _placeableObject = (_currentPlaceable as MonoBehaviour)?.gameObject;

            if (_placeableObject == null)
            {
                Debug.LogWarning($"placeable is not gameObject : {_currentPlaceable}");
                _currentPlaceable = null;
                return;
            }

            _placeableObject.transform.rotation = _prevTargetRotation;
            _currentPlaceable.StartPlace();

            _placePos = _placeableObject.transform.position.ToVector3Int();
            _placeableObject.transform.position = _placePos;

            _offset = Mathf.CeilToInt(_placeableObject.transform.localScale.x) % 2 == 0
                ? Vector3.zero
                : Vector3.one.RemoveY();
        }

        private void HandlePlaceKeyPressed(bool isStarted) => _isPlaceKeyHold = isStarted;

        private void TryToPlace()
        {
            if (_currentPlaceable == null) return;

            Vector3Int placeableObjectPos = _placeableObject.transform.position.ToVector3Int();
            
            if (_currentPlaceable.TryPlace(placeableObjectPos) == false) return;

            IsPlanting = false;

            _rotateTween?.Complete();
            PlaceObject(placeableObjectPos);
            _prevTargetRotation = _placeableObject.transform.rotation;

            systemChannel.RaiseEvent(SystemEvents.PlaceCompleteEvent.Initializer(_currentPlaceable));
            
            if (_currentPlaceable is Entity entity)
            {
                ResetPlaceable();
                entity.OnPlaceEvent?.Invoke(entity);
            }
            else
            {
                ResetPlaceable();
            }
            
            systemChannel.RaiseEvent(SystemEvents.BakeMapEvent);
        }

        private void PlaceObject(Vector3Int pos)
        {
            _currentPlaceable?.CompletePlace(pos);
        }

        private void ResetPlaceable()
        {
            _currentPlaceable = null;
            _placeableObject = null;
        }

        private void HandleRotateKeyPressed()
        {
            if (_currentPlaceable == null) return;

            _rotateTween?.Complete();

            Vector3 currentRot = _placeableObject.transform.eulerAngles;
            Vector3 targetRot = new Vector3(currentRot.x, currentRot.y + 90, currentRot.z);

            _rotateTween = _placeableObject.transform.DORotate(targetRot, rotateDuration);
        }

        private void Update()
        {
            RaycastHit hit = playerInput.GroundHit;

            if (_currentPlaceable == null || playerInput.GroundHitValid == false) return;

            _placePos = GetGridPos(hit.point.ToVector3Int());
            _placeableObject.transform.position = _placePos + _offset;
            
            if(_isPlaceKeyHold == false) return;
            
            TryToPlace();
        }

        private Vector3Int GetGridPos(Vector3 worldPosition)
        {
            int y = Mathf.FloorToInt(worldPosition.y / 2f) * 2;
            int z = Mathf.FloorToInt(worldPosition.z / 2f) * 2;
            int x = Mathf.FloorToInt(worldPosition.x / 2f) * 2;

            return new Vector3Int(x, y, z);
        }
    }
}