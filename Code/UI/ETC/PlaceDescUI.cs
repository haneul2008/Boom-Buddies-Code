using Code.Managers;
using HNLib.Dependencies;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.ETC
{
    public class PlaceDescUI : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        
        [Inject] private PlaceManager _placeManager;

        private void Update()
        {
            if (background.activeSelf != _placeManager.IsPlanting)
            {
                background.SetActive(_placeManager.IsPlanting);
            }
        }
    }
}