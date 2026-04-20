using Code.Managers;
using Code.Stages;
using UnityEngine;

namespace Code.EventSystems
{
    public static class StageEvents
    {
        public static readonly SetStageEvent SetStageEvent = new SetStageEvent();
        public static readonly StageClearEvent StageClearEvent = new StageClearEvent();
    }

    public class SetStageEvent : GameEvent
    {
        public StageDataSO stageData;
        public bool isFirstClear;
        public bool isFriendlyMatch;
        public FriendlyMatchData matchData;

        public SetStageEvent Initializer(StageDataSO stageData, bool isFirstClear, bool isFriendlyMatch, FriendlyMatchData matchData)
        {
            this.stageData = stageData;
            this.isFirstClear = isFirstClear;
            this.isFriendlyMatch = isFriendlyMatch;
            this.matchData = matchData;
            return this;
        }
    }

    public class StageClearEvent : GameEvent
    {
        public StageDataSO stageData;

        public StageClearEvent Initializer(StageDataSO stageData)
        {
            this.stageData = stageData;
            return this;
        }
    }
}