using System;
using UnityEngine;

namespace Npu
{
    public class ObjectDataProvider : MonoBehaviour, IDataProvider
    {
        public event Action<IDataProvider, int> DataChanged;

        [SerializeField] private UnityEngine.Object data;

        public UnityEngine.Object Data
        {
            get => data;

            set
            {
                data = value;
                DataChanged?.Invoke(this, DataBinder.FlagAll);
            }
        }

        public object GetData()
        {
            return data;
        }

        public bool Ready => true;
        
        public Type GetDataType()
        {
            return data?.GetType();
        }

        static string[] _bindingFilters = {"default"};
        public string[] BindingFilters => _bindingFilters;
    }

}