using System;

namespace Npu.Common
{
    public class AbstractLazyValue<TData>
    {
        private readonly Func<TData> getter;
        private readonly Action<TData> setter;

        public event Action<TData> Changed;  

        public AbstractLazyValue(Func<TData> getter, Action<TData> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        private bool hasValue;
        private TData value;
        public TData Value
        {
            get
            {
                if (hasValue) return value;
                value = getter(); 
                hasValue = true;
                return value;
            }
            set
            {
                hasValue = true;
                this.value = value;
                setter.Invoke(value);
                Changed?.Invoke(value);
            }
        }

        public static implicit operator TData(AbstractLazyValue<TData> v) => v.Value;

        public void Reset()
        {
            hasValue = false;    
        }
    }

    public class PlayerPrefsValue<TData> : AbstractLazyValue<TData>
    {
        public PlayerPrefsValue(string key, Func<string, TData, TData> getter, Action<string, TData> setter, TData defaultValue) :
        base(() => getter.Invoke(key, defaultValue), v => setter.Invoke(key, v)) { }
    }
}