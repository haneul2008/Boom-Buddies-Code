using System;
using Code.Input;
using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(fileName = "SelectData", menuName = "SO/SelectData", order = 0)]
    public class SelectDataSO : ScriptableObject
    {
        public event Action<RaycastHit> OnSelect;

        [SerializeField] private PlayerInputSO playerInput;

        private RaycastHit _hit;

        public void Initialize()
        {
            playerInput.OnMouseLeftPressed += HandleMouseLeftPressed;
        }

        public void Disable()
        {
            playerInput.OnMouseLeftPressed -= HandleMouseLeftPressed;
        }

        private void HandleMouseLeftPressed(bool isStarted)
        {
            if(isStarted == false || playerInput.TargetHitValid == false) return;

            _hit = playerInput.TargetHit;
            OnSelect?.Invoke(_hit);
        }
    }
}