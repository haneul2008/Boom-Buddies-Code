using System.Collections.Generic;
using UnityEngine;

namespace Code.Stages
{
    [CreateAssetMenu(fileName = "StageList", menuName = "SO/Stage/List", order = 0)]
    public class StageListSO : ScriptableObject
    {
        public List<StageDataSO> stageDataList;
    }
}