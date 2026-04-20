using UnityEngine;

namespace Code.UI.Stats
{
    [CreateAssetMenu(fileName = "StatUiSO", menuName = "SO/UI/StatUIData", order = 0)]
    public class StatUIDataSO : ScriptableObject
    {
        public StatEnum statEnum;
        public string uiTitle;
        public Sprite icon;
        public Vector2 iconSize;
    }
}