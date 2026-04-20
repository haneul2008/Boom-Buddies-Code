using UnityEngine;

namespace Code.Core
{
    public interface IPlaceable
    {
        public bool IsPlaced { get;}
        public void StartPlace();
        public void CompletePlace(Vector3 pos);
        public bool TryPlace(Vector3Int pos);
    }
}