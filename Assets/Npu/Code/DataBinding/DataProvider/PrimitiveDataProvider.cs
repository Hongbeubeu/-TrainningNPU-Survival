using System;
using Npu.Core;
using UnityEngine;

namespace Npu
{
    public class PrimitiveDataProvider : MonoBehaviour, IDataProvider
    {
        public event Action<IDataProvider, int> DataChanged;

        public int anInt;
        public float aFloat;
        public double aDouble;
        public bool aBool;

        [Space] public Vector2 vector2;
        public Vector3 vector3;
        public Vector4 vector4;

        [Space] public SecuredInt securedInt;

        public SecuredLong securedLong;

        //public SecuredFloat securedFloat;
        public SecuredDouble securedDouble;

        [Space] public string aString;
        public char aChar;

        public object GetData() => this;
        public bool Ready => true;
        public Type GetDataType() => GetType();

        static string[] _bindingFilters = {"default"};
        public string[] BindingFilters => _bindingFilters;

        [ContextMenu("Trigger Changed Event")]
        public void TriggerChanged() => DataChanged?.Invoke(this, DataBinder.FlagAll);

        public void OnValidate() => TriggerChanged();
    }

}