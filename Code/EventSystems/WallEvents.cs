using System.Collections.Generic;
using Code.Walls;
using UnityEngine;

namespace Code.EventSystems
{
    public static class WallEvents
    {
        public static readonly CheckNearWallEvent CheckNearWallEvent = new CheckNearWallEvent();
    }

    public class CheckNearWallEvent : GameEvent
    {
        public Dictionary<Vector3Int, Wall> wallPairs;

        public CheckNearWallEvent Initializer(Dictionary<Vector3Int, Wall> wallPairs)
        {
            this.wallPairs = wallPairs;
            return this;
        }
    }
}