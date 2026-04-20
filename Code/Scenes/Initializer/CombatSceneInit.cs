using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Managers;
using HNLib.Dependencies;
using UnityEngine;

namespace Code.Scenes.Initializer
{
    public class CombatSceneInit : MonoBehaviour, ISceneInit
    {
        public int Priority => 10;
        
        public void OnSceneInit()
        {
            SetCombatStage(true);
        }

        public void OnSceneExit()
        {
            SetCombatStage(false);
        }

        private void SetCombatStage(bool isCombatStage)
        {
            List<ICombatStageBehavior> stageBehaviors = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ICombatStageBehavior>().ToList();
            
            stageBehaviors.ForEach(stageBehavior => stageBehavior.IsCombatStage = isCombatStage);
        }
    }
}