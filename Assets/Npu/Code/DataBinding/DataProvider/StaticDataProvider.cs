using UnityEngine;
using System;

namespace Npu
{
    public abstract class StaticDataProvider : MonoBehaviour, IDataProvider
    {
        public event Action<IDataProvider, int> DataChanged;


        public object GetData()
        {
            return null;
        }

        public bool Ready => true;
        public abstract Type GetDataType();

        static string[] _bindingFilters = {"default"};
        public string[] BindingFilters => _bindingFilters;
    }

}