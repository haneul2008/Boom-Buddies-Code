using System.Collections.Generic;
using System.Linq;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.UI.ETC
{
    [Provide]
    public class UIPointerChecker : MonoBehaviour, IDependencyProvider
    {
        private List<GameCanvas> _canvasList;

        private void Awake()
        {
            _canvasList = FindObjectsByType<GameCanvas>(FindObjectsSortMode.None).ToList();
        }

        public bool IsPointerOverUI(Vector2 screenPos)
        {
            foreach (GameCanvas canvas in _canvasList)
            {
                if (canvas.IsPointerOverUI(screenPos)) return true;
            }

            return false;
        }
    }
}