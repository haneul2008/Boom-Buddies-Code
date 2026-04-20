using System;
using Code.EventSystems;
using Code.Scenes.Initializer;
using HNLib.Dependencies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Managers
{
    [Provide]
    public class GoldManager : MonoBehaviour, IDependencyProvider, IOnceManager
    {
        public int Priority => 0;
        public int CurrentGold { get; private set; }

        [SerializeField] private GameEventChannelSO goldChannel;
        [SerializeField] private int initGold;

        [Inject] private SaveManager _saveManager;
        
        private int _addGoldStorage;
        
        public void Initialize()
        {
            goldChannel.AddListener<GoldChangeEvent>(HandleGoldChange);
            goldChannel.AddListener<GoldSetEvent>(HandleGoldSet);
            goldChannel.AddListener<SetAddGoldEvent>(HandleAddGold);
            
            if(_saveManager.IsDataEmpty())
                CurrentGold = initGold;
        }

        private void OnDestroy()
        {
            goldChannel.RemoveListener<GoldChangeEvent>(HandleGoldChange);
            goldChannel.RemoveListener<GoldSetEvent>(HandleGoldSet);
            goldChannel.RemoveListener<SetAddGoldEvent>(HandleAddGold);
        }

        private void HandleAddGold(SetAddGoldEvent evt) => _addGoldStorage = evt.gold;

        private void HandleGoldSet(GoldSetEvent evt)
        {
            CurrentGold = evt.gold + _addGoldStorage;
            _addGoldStorage = 0;
        }

        private void HandleGoldChange(GoldChangeEvent evt)
        {
            if (CurrentGold < -evt.gold)
            {
                evt.onChangeGold?.Invoke(false);
            }
            else
            {
                CurrentGold += evt.gold;
                evt.onChangeGold?.Invoke(true);
            }
        }
    }
}