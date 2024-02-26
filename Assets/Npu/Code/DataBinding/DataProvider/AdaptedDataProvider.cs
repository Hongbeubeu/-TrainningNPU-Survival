using System;
using Npu.Common;
using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;
using Npu.Helper;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Npu
{
    public class AdaptedDataProvider : MonoBehaviour, IDataProvider
    {
        [SerializeField, TypeSelector(typeof(Object))] protected string type;
        
        
        private object _data;
        public object Data
        {
            get => _data;
            set
            {
                _data = value;
                Ready = _data != null && DataType.IsInstanceOfType(_data);
                if (Ready) TriggerChanged(~0);
                if (!Ready) Logger.Error(this, GetType().Name, $"Invalid value (expect {DataType}, has {_data?.GetType()})");
            }
        }

        private Type _dataType;
        private Type DataType => _dataType ?? (_dataType = ReflectionUtils.GetType(type));
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _dataType = null;
        }
#endif

    #region IDataProvider
        
        public event Action<IDataProvider, int> DataChanged;
        public bool Ready { get; protected set; }
        public Type GetDataType() => DataType;
        public object GetData() => Data;
        private static string[] _bindingFilters = {"default"};
        public string[] BindingFilters => _bindingFilters;
    
        protected void TriggerChanged(int mask) => DataChanged?.Invoke(this, mask);
        protected void TriggerChanged(string mask) => DataChanged?.Invoke(this, this.GetFilters(mask));
        
        
        #if UNITY_EDITOR

        [InspectorGUI(group = "DataBinder Trigger", gui = true, expand = 5, runtimeOnly = true)]
        private void InvokeChangedGUI()
        {
            using (new HorizontalLayout())
            {
                foreach (var i in BindingFilters)
                {
                    if (GUILayout.Button(i)) TriggerChanged(i);
                    
                }
                
                if (GUILayout.Button("All")) TriggerChanged(~0);
            }
        }
        
        #endif
        
    #endregion    

    }
}