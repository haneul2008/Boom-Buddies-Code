using System.Collections.Generic;
using UnityEngine;

namespace Code.References.Text
{
    [CreateAssetMenu(fileName = "TextData", menuName = "SO/Text/TextData", order = 0)]
    public class TextDataSO : ScriptableObject
    {
        public string key;
        public List<string> text;
        public string color;
        
        public virtual object GetKey()
        {
            return key;
        }
    }
}