using System;
using UnityEngine;

namespace Code.Stages
{
    [CreateAssetMenu(fileName = "StageData", menuName = "SO/Stage/StageData", order = 0)]
    public class StageDataSO : ScriptableObject
    {
        public int stageNum;
        public int difficult;
        public int firstClearReward;
        public int reward;
        public int costLimit;
    }
}