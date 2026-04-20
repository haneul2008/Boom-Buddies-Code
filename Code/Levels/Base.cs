using System;
using System.Collections.Generic;
using System.Linq;
using Code.EventSystems;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Levels
{
    public class Base : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surface;
        [SerializeField] private GameEventChannelSO systemChannel;

        private void Awake()
        {
            systemChannel.AddListener<BakeMapEvent>(HandleBakeMap);
        }

        private void OnDestroy()
        {
            systemChannel.RemoveListener<BakeMapEvent>(HandleBakeMap);
        }

        private void HandleBakeMap(BakeMapEvent evt)
        {
            surface.BuildNavMesh();
        }
    }
}