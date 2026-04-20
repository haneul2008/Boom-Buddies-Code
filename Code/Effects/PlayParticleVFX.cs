using System;
using UnityEngine;

namespace Code.Effects
{
    public class PlayParticleVFX : MonoBehaviour, IPlayableVFX
    {
        public float Duration { get; private set; }
        [field: SerializeField] public string VFXName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private ParticleSystem particle;

        private void Awake()
        {
            Duration = particle.main.duration + 1;
        }

        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            if (isOnPosition == false)
                transform.SetPositionAndRotation(position, rotation);
            
            particle.Play(true);
        }

        public void StopVFX()
        {
            particle.Stop(true);
        }

        private void OnValidate()
        {
            gameObject.name = VFXName;
        }
    }
}