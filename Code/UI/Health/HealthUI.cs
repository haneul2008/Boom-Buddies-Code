using System;
using Code.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Health
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private bool isEntityRotate;

        private EntityHealth _health;
        private Transform _parentTrm;
        private float _beforeY;

        private void Start()
        {
            Entity entity = GetComponentInParent<Entity>();
            _parentTrm = entity.transform;
            
            _health = entity.GetCompo<EntityHealth>();
            _health.OnHit.AddListener(HandleHit);
            _health.OnDead.AddListener(HandleDead);

            transform.rotation = Camera.main.transform.rotation;
            
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if(isEntityRotate == false && Mathf.Approximately(_beforeY, _parentTrm.eulerAngles.y)) return;
            
            transform.rotation = Camera.main.transform.rotation;
            _beforeY = _parentTrm.eulerAngles.y;
        }
        
        private void HandleDead(Entity entity)
        {
            _health.OnHit.RemoveListener(HandleHit);
            _health.OnDead.RemoveListener(HandleDead);

            gameObject.SetActive(false);
        }

        private void HandleHit(float current)
        {
            if(gameObject.activeSelf == false) gameObject.SetActive(true);

            fillImage.fillAmount = current / _health.MaxHealth;
        }
    }
}