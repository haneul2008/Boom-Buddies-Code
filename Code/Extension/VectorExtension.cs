using UnityEngine;

namespace Code.Extension
{
    public static class VectorExtension
    {
        public static bool IsEquals(this Vector2 vec1, Vector2 vec2)
        {
            return Mathf.Approximately(vec1.x, vec2.x) && Mathf.Approximately(vec1.y, vec2.y);
        }
        
        public static bool IsEquals(this Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Approximately(vec1.x, vec2.x) && Mathf.Approximately(vec1.y, vec2.y) && Mathf.Approximately(vec1.z, vec2.z);
        }

        public static Vector3Int ToVector3Int(this Vector3 vec)
        {
            return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        }

        public static Vector3 RemoveY(this Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }
    }
}