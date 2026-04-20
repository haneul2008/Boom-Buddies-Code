using System.Collections;
using Code.Entities;
using Code.EventSystems;
using Code.Managers;
using Code.Units;
using TMPro;
using UnityEngine;

namespace Code.UI.Units
{
    public class UnitSpawnableUI : UnitUI
    {
        [SerializeField] private GameEventChannelSO systemChannel;
        [SerializeField] private GameEventChannelSO unitChannel;
        [SerializeField] private TextMeshProUGUI costText;
        
        private UnitManager _unitManager;
        private PlaceManager _placeManager;
        private bool _isSpawnedAllUnit;

        public override void Initialize(UnitDataSO unitData)
        {
            base.Initialize(unitData);

            _unitManager = CreateOnceManager.Instance.GetManager<UnitManager>();
            _placeManager = CreateOnceManager.Instance.GetManager<PlaceManager>();
            systemChannel.AddListener<PlaceStartEvent>(HandlePlaceStart);
            costText.text = unitData.cost.ToString();
        }

        private void OnDestroy()
        {
            systemChannel.RemoveListener<PlaceStartEvent>(HandlePlaceStart);
        }

        private void HandlePlaceStart(PlaceStartEvent evt)
        {
            if(evt.placeable is Unit unit == false) return;
            
            unit.OnPlaceEvent.AddListener(HandlePlaceComplete);
        }

        private void HandlePlaceComplete(Entity entity)
        {
            if (entity is Unit unit && unit.SpawnableData == _unitData)
            {
                SetAmountText(--Amount);
                
                unitChannel.RaiseEvent(UnitEvents.UnitSpawnEvent.Initializer(unit.SpawnableData as UnitDataSO));

                if (Amount == 0)
                {
                    _isSpawnedAllUnit = true;
                    StartCoroutine(CancelPlaceCoroutine());
                    unitChannel.RaiseEvent(UnitEvents.CheckLastUnitSpawnEvent);
                }
            }
        }

        private IEnumerator CancelPlaceCoroutine()
        {
            yield return new WaitForEndOfFrame();
            _placeManager.CancelPlace();
        }

        public void SpawnUnit()
        {
            if(_isSpawnedAllUnit) return;
            
            _unitManager.Create(_unitData);
        }
    }
}