using System;
using Npu.Core;
using UnityEngine;

namespace Npu.Formula
{

    public interface IParameter
    {
        event Action<IParameter> Changed;
        SecuredDouble Value { get; set; }
        string Name { get; set; }
        string Description { get; }
    }

    public abstract class AbstractParameter : IParameter
    {
        public event Action<IParameter> Changed = delegate { };

        protected SecuredDouble value;

        protected AbstractParameter()
        {
            this.value = 0;
        }

        protected AbstractParameter(SecuredDouble value)
        {
            this.value = value;
        }

        protected AbstractParameter(string name)
        {
            this.Name = name;
        }

        protected AbstractParameter(string name, SecuredDouble value)
        {
            this.Name = name;
            this.value = value;
        }

        public string Name { get; set; }
        
        protected void OnChanged()
        {
            Changed.Invoke(this);
        }

        public abstract SecuredDouble Value { get; set; }

        public virtual string Description => $"{Name} ({GetType()})";
    }

    public class Parameter : AbstractParameter
    {

        public Parameter(string name, SecuredDouble value) : base(name, value)
        {

        }

        public override SecuredDouble Value
        {
            get => value;
            set
            {
                this.value = value;
                OnChanged();
            }
        }
    }

    public class BoundParameter : AbstractParameter
    {

        protected SecuredDouble minValue;
        protected SecuredDouble maxValue;

        public BoundParameter()
        {
            this.minValue = 0;
            this.maxValue = 1;
            this.value = minValue;
        }

        public BoundParameter(string name, SecuredDouble minValue, SecuredDouble maxValue) : base(name, minValue)
        {
            this.minValue = minValue;
            this.maxValue = SecuredDouble.Max(minValue, maxValue);
        }

        public override SecuredDouble Value
        {
            get => value;
            set
            {
                this.value = SecuredDouble.Clamp(value, minValue, maxValue);
                OnChanged();
            }
        }
    }

    public class ConstantParameter : AbstractParameter
    {
        public ConstantParameter(string name, SecuredDouble value) : base(name, value)
        {

        }
        public ConstantParameter(SecuredDouble value) : base(value)
        {
        }

        public override SecuredDouble Value
        {
            get => value;
            set
            {
                Debug.LogErrorFormat("Cannot change value of {0}", GetType());
                OnChanged();
            }
        }
    }

    public class BridgeParameter : IParameter
    {
        public event Action<IParameter> Changed = delegate { };

        private IParameter source;
        private readonly SecuredDouble defaultValue;

        public BridgeParameter(string name, IParameter source, double defaultValue)
        {
            Name = name;
            Source = source;
            this.defaultValue = defaultValue;
        }
        
        public BridgeParameter(string name, double defaultValue)
        {
            Name = name;
            this.defaultValue = defaultValue;
        }

        public string Name { get; set; }
        public virtual string Description => $"{Name} ({GetType()}, source={Source?.Name})";

        public IParameter Source
        {
            get => source;
            set
            {
                if (source != null)
                {
                    source.Changed -= OnSourceChanged;
                }

                source = value;
                if (source != null) source.Changed += OnSourceChanged;
            }
        }

        public void Connect(IParameter source) => Source = source;
        public void Disconnect() => Source = null;
        

        private void OnSourceChanged(IParameter parameter)
        {
            Changed?.Invoke(this);
        }
        
        public virtual SecuredDouble Value
        {
            get => Source?.Value ?? defaultValue;
            set { }
        }
    }

    public class ClampedParameter : BridgeParameter
    {
        public SecuredDouble Min { get; set; }
        public SecuredDouble Max { get; set; }

        public ClampedParameter(string name, IParameter source, SecuredDouble min, SecuredDouble max) : base(name, source, 0)
        {
            Min = min;
            Max = max;
        }

        public override SecuredDouble Value
        {
            get => SecuredDouble.Clamp(base.Value, Min, Max);
            set => Source.Value = value;
        }

        public override string Description => $"{Name} ({GetType()}, source={Source.Name}, min={Min}, max={Max})";
    }

    public class ParameterValve
    {
        public IParameter Source { get; private set; }
        public IParameter Target { get; private set; }

        private SecuredDouble lastTargetValue;
        
        public ParameterValve(IParameter source, IParameter target)
        {
            this.Source = source;
            this.Target = target;
        }

        public void Block(SecuredDouble lastDrop)
        {
            Source.Changed -= OnSourceChanged;

            Target.Value = lastDrop;
        }
        
        public void Block()
        {
            Block(lastTargetValue);
        }

        public void Release()
        {
            lastTargetValue = Target.Value;
            
            Source.Changed -= OnSourceChanged;
            Source.Changed += OnSourceChanged;

            OnSourceChanged(Source);
        }

        private void OnSourceChanged(IParameter p)
        {
            Target.Value = Source.Value;
        }
    }
}