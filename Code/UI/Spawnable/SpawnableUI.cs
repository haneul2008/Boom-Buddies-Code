using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Managers;
using Code.UI.ETC;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.UI.Spawnable
{
    public class SpawnableUI : MonoBehaviour
    {
        [SerializeField] private SelectDataSO selectData;
        
        private readonly Dictionary<SpawnableDataSO, SpawnableInfoPanel> _infoPanelPairs = new Dictionary<SpawnableDataSO, SpawnableInfoPanel>();
        private readonly Dictionary<Collider, ISpawnable> _spawnablePairs = new Dictionary<Collider, ISpawnable>();
        
        [Inject] private PlaceManager _placeManager;
        [Inject] private UIPointerChecker _pointerChecker;

        private void Awake()
        {
            InitPanels();

            selectData.OnSelect += HandleSelect;
        }

        private void InitPanels()
        {
            GetComponentsInChildren<SpawnableInfoPanel>().ToList().ForEach(panel =>
            {
                foreach (SpawnableDataSO data in panel.TargetDataList)
                {
                    if (_infoPanelPairs.TryAdd(data, panel) == false)
                        Debug.LogWarning($"Key duplicate {panel.gameObject.name}");
                }
                
                panel.Initialize();
                panel.SetActive(null,false);
            });
        }

        public void ActiveFalse()
        {
            foreach (SpawnableInfoPanel panel in _infoPanelPairs.Values)
            {
                panel.SetActive(null, false);
            }
        }

        private void OnDestroy()
        {
            selectData.OnSelect -= HandleSelect;
        }

        private void HandleSelect(RaycastHit hit)
        {
            if(hit.collider == null || _pointerChecker.IsPointerOverUI(Camera.main.WorldToScreenPoint(hit.point))) return;
            
            if (_spawnablePairs.TryGetValue(hit.collider, out ISpawnable spawnable) == false)
            {
                if (hit.collider.TryGetComponent(out spawnable))
                {
                    _spawnablePairs.Add(hit.collider, spawnable);
                }
            }
            
            if(spawnable == null || _placeManager.IsPlanting) return;
            
            Select(spawnable);
        }

        public void Select(ISpawnable spawnable)
        {
            if (_infoPanelPairs.TryGetValue(spawnable.SpawnableData, out SpawnableInfoPanel infoPanel))
            {
                ActivePanels(false);
                infoPanel.SetActive(spawnable, true);
            }
        }

        private void ActivePanels(bool isActive)
        {
            foreach (SpawnableInfoPanel panel in _infoPanelPairs.Values)
            {
                panel.SetActive(null, isActive);
            }
        }
    }
}