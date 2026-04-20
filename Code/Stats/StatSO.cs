using System;
using System.Collections.Generic;
using Code.Entities;
using UnityEngine;

namespace Code.Stats
{
    [CreateAssetMenu(fileName = "Stat", menuName = "SO/Stat", order = 0)]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChanged(StatSO stat, float prev, float current);
        public event ValueChanged OnValueChanged;
        
        public float Value => Mathf.Clamp(_baseValue  + _modifiedValue, minValue, maxValue);

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                float prevValue = Value;
                _baseValue = Mathf.Clamp(value, minValue, maxValue); //들어온 값을 clamp
                InvokeValueChange(Value, prevValue);
            }
        }
        
        public string statName;
        public float minValue, maxValue;
        public Sprite statSprite;
        private float _modifiedValue;
        private float _baseValue;
        
        private Dictionary<object, float> _modifyPairs = new Dictionary<object, float>();
        
        public void AddModifier(object key, float value)
        {
            if (_modifyPairs.ContainsKey(key)) return;

            float prev = Value;
            
            _modifiedValue += value;
            _modifyPairs.Add(key, value);
            InvokeValueChange(prev, Value);
        }

        public void RemoveModifier(object key)
        {
            if (!_modifyPairs.ContainsKey(key)) return;

            float prev = Value;

            _modifiedValue -= _modifyPairs[key];
            _modifyPairs.Remove(key);
            
            InvokeValueChange(prev, Value);
        }

        private void InvokeValueChange(float prev, float current)
        {
            if(Mathf.Approximately(prev, current)) return;

            OnValueChanged?.Invoke(this, prev, current);
        }

        public object Clone() => Instantiate(this);

        public List<EntityStatCompo.ModifyData> GetModifyData()
        {
            List<EntityStatCompo.ModifyData> res = new List<EntityStatCompo.ModifyData>();
            
            foreach (var pair in _modifyPairs)
            {
                EntityStatCompo.ModifyData modifyData;
                modifyData.key = pair.Key.ToString();
                modifyData.value = pair.Value;
                
                res.Add(modifyData);
            }

            return res;
        }

        [ContextMenu("PrintModifiers")]
        public void PrintModifiers()
        {
            foreach (var pair in _modifyPairs)
            {
                Debug.Log($"Key : {pair.Key.ToString()}, value : {pair.Value}");
            }
        }
    }
}