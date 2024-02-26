using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Npu.Helper
{
    [CreateAssetMenu]
    public class ManualValueTrigger : ScriptableObject, IValueTrigger
    {
        [SerializeField, Box] public List<Value> values;
    
    
        public event Action<int, double> ValueChanged;

        public string[] CategoryNames => values.Select(i => i.category).Distinct().OrderBy(i => i).ToArray();

        public double GetValue(int category)
        {
            var categoryName = CategoryNames[category];
            return values.FirstOrDefault(i => i.category == categoryName)?.value ?? -1;
        }

        public string GetDescription(int category, double value) => $"Category {category}: {value}";
        public (bool, string) Validate(int category, double value) => (true, "Just Good");

        [ContextMenu("Trigger All")]
        public void TriggerAll()
        {
            var cats = CategoryNames;
            for (var i = 0; i < cats.Length; i++)
            {
                ValueChanged?.Invoke(i, values.FirstOrDefault(v => v.category == cats[i]).value);
            }
        }

        [Serializable]
        public class Value
        {
            public string category;
            public double value;
        }
    }
}

