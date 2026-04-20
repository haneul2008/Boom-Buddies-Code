using UnityEngine;

namespace Code.Core
{
    public interface IPlaceMediator
    {
        bool TryPlace(Vector3Int pos, IPlaceable placeable);
    }
}