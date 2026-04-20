using Code.Managers;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Scenes.Initializer
{
    public class BaseSceneInit : MonoBehaviour, ISceneInit
    {
        public int Priority => 10;
        
        public void OnSceneInit()
        {
            
        }

        public void OnSceneExit()
        {
            
        }
    }
}