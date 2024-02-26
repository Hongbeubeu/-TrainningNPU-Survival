using System;
using System.Linq;
using Npu.Helper;
using UnityEngine;

namespace Npu.Common
{
    [CreateAssetMenu(fileName = "CommonKeyedValues")]
    public class CommonKeyedValues : ScriptableObject, IKeyedValueProvider
    {

        [SerializeField, Box] private DoubleValue[] doubleValues;
        [SerializeField, Box] private StringValue[] stringValues;
        
        #region IKeyedValueProvider

        public string[] Keys => doubleValues.Select(i => i.key).Concat(stringValues.Select(i => i.key)).ToArray();
        
        public object Get(string key)
        {
            if (doubleValues.TryGet(i => i.key.Equals(key), out var v)) return v.value;
            if (stringValues.TryGet(i => i.key.Equals(key), out var vv)) return vv.value;
            
            throw new ArgumentException($"Key {key} not found");
        }
        
        #endregion
        
        [Serializable]
        public class DoubleValue
        {
            public string key;
            public double value;
        }
        
        [Serializable]
        public class StringValue
        {
            public string key;
            public string value;
        }
        
        [Serializable]
        public class IntValue
        {
            public string key;
            public int value;
        }
        
        
    }
}