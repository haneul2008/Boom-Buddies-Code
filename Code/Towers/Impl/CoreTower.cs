using Code.Entities;
using Code.EventSystems;
using UnityEngine;

namespace Code.Towers.Impl
{
    public class CoreTower : Tower
    {
        [SerializeField] private GameEventChannelSO uiChannel;

        private bool _isActiveHealth;
        
        public override void Spawn()
        {
        }
        
        public override void HandleDead()
        {
            IsDead = true;
            _collider.enabled = false;
            IsAutoRotate = false;
            
            uiChannel.RaiseEvent(UIEvents.PopUpResultUIEvent.Initializer(true));
            
            Destroy(gameObject);
        }

        public void ActiveHealth() => _isActiveHealth = true;

        public override void ApplyDamage(float damage, Vector3 hitPoint, Vector3 normal, Entity dealer)
        {
            if(_isActiveHealth == false) return;
            
            base.ApplyDamage(damage, hitPoint, normal, dealer);
        }
    }
}