using UnityEngine;

namespace Code.Scenes
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "SO/SceneData", order = 0)]
    public class SceneDataSO : ScriptableObject
    {
        public string sceneName;
        public bool isSaveData;
    }
}