using System;
using Code.Managers;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Levels
{
    public class NotSpawnableArea : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        [Inject] private PlaceManager _placeManager;

        private void Update()
        {
            if (meshRenderer.enabled != _placeManager.IsPlanting)
            {
                meshRenderer.enabled = _placeManager.IsPlanting;
            }
        }
    }
}