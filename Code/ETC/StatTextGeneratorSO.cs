using System;
using System.Collections.Generic;
using Code.References.Text;
using Code.Stats;
using Code.UI;
using Code.UI.Stats;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.ETC
{
    [Serializable]
    public struct StatPair
    {
        public StatEnum statEnum;
        public StatSO statSO;
    }

    [CreateAssetMenu(fileName = "StatTextGenerator", menuName = "SO/StatTextGenerator", order = 0)]
    public class StatTextGeneratorSO : ScriptableObject
    {
        [SerializeField] private List<StatPair> statPairs;
        [SerializeField] private TextContainerSO textContainer;
        [SerializeField] private string path;

#if UNITY_EDITOR
        [ContextMenu("Generate")]
        public void GenerateText()
        {
            foreach (StatPair statPair in statPairs)
            {
                string statName = statPair.statSO.statName;
                
                TextDataSO textData = ScriptableObject.CreateInstance<TextDataSO>();
                textData.name = $"{statName}_TextData";
                textData.key = statName;
                textData.text = new List<string>() { statPair.statEnum.ToString() };
                
                string assetPath = $"{path}/{textData.name}.asset";
                AssetDatabase.CreateAsset(textData, assetPath);
                
                TextDataSO savedData = AssetDatabase.LoadAssetAtPath<TextDataSO>(assetPath);
                
                if(textContainer.Contains(savedData)) continue;
                
                textContainer.AddData(savedData);
                EditorUtility.SetDirty(textContainer);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}