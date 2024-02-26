using System;
using System.Reflection;

namespace Npu
{
    /// <summary>
    /// Facade to a <see cref="MemberInfo"/> (either <see cref="PropertyInfo"/>, <see cref="FieldInfo"/> or <see cref="MethodInfo"/>)
    /// </summary>
    public class MemberInfoFacade
    {
        // For optimization purpose
        private static object[] _dummyObjectArr0 = new object[0];
        private static object[] _dummyObjectArr1 = new object[1];

        private PropertyInfo _propertyInfo;
        private FieldInfo _fieldInfo;
        private MethodInfo _methodInfo;
        private string _member;

        public bool IsStatic { get; private set; }
        public bool IsValid { get; private set; }
        public Type ReturnType { get; private set; }


        public void Init(Type type, string member, UnityEngine.Object context=null)
        {
            _member = member;
            _fieldInfo = null;
            _propertyInfo = null;
            _methodInfo = null;
            IsValid = false;
            ReturnType = null;

            if (type == null || string.IsNullOrEmpty(member)) return;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            _fieldInfo = type.GetField(member, flags);
            if (_fieldInfo == null)
            {
                _propertyInfo = type.GetProperty(member, flags);
            }
            if (_fieldInfo == null && _propertyInfo == null)
            {
                try
                {
                    _methodInfo = type.GetMethod(member, flags);
                }
                catch (Exception ex)
                {
                    Logger.Error<MemberInfoFacade>(context, $"{type} / {member} / {ex}");
                }

            }

            IsStatic = _fieldInfo?.IsStatic ?? _propertyInfo?.GetMethod?.IsStatic ?? _methodInfo?.IsStatic ?? false;
            IsValid = _fieldInfo != null || _propertyInfo != null || _methodInfo != null;
            ReturnType = _fieldInfo?.FieldType ?? _propertyInfo?.PropertyType ?? _methodInfo?.ReturnType;
        }


        public object GetValue(object @object)
        {
            if (@object == null && !IsStatic)
            {
                return null;
            }

            if (_fieldInfo != null)
            {
                return _fieldInfo.GetValue(@object);
            }

            if (_propertyInfo != null)
            {
                return _propertyInfo.GetValue(@object);
            }
                
            return _methodInfo != null ? _methodInfo.Invoke(@object, _dummyObjectArr0) : default;
        }

        public void SetValue(object @object, object value)
        {
            if (_fieldInfo != null)
            {
                _fieldInfo.SetValue(@object, value);
            }
            else if (_propertyInfo != null)
            {
                _propertyInfo.SetValue(@object, value);
            }
            else if (_methodInfo != null)
            {
                _dummyObjectArr1[0] = value;
                _methodInfo.Invoke(@object, _dummyObjectArr1);
            }
        }

        public void Invoke(object @object)
        {
            _methodInfo?.Invoke(@object, _dummyObjectArr0);
        }

        public override string ToString()
        {
            return string.Format("Member Name: {0}\n Member: {1} ({2}) ({3})",
                _member,
                _fieldInfo?.Name ?? _propertyInfo?.Name ?? _methodInfo?.Name,
                _fieldInfo?.FieldType ?? _propertyInfo?.PropertyType ?? _methodInfo?.ReturnType,
                _fieldInfo?.GetType() ?? _propertyInfo?.GetType() ?? _methodInfo?.GetType());
        }
    }
}