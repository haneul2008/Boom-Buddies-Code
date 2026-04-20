using Code.EventSystems;
using Code.Scenes;
using Code.Stages;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Tester
{
    public class CombatTester : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO stageChannel;
        [SerializeField] private GameEventChannelSO sceneChannel;
        [SerializeField] private SceneDataSO combatSceneData;
        [SerializeField] private StageDataSO targetStage;

        [ContextMenu("Set Stage")]
        public void SetStage()
        {
            sceneChannel.RaiseEvent(SceneEvents.SceneChangeEvent.Initializer(combatSceneData));
            stageChannel.RaiseEvent(StageEvents.SetStageEvent.Initializer(targetStage, true, false, null));
        }
    }
}