using UnityEngine;
using System;
using System.Reflection;
using Npu.Core;


namespace Npu.Formula
{
    public class InnerParameterAttribute : Attribute
    {
    }
    
    public class InjectParameterAttribute : Attribute
    {
        public string name;

        public InjectParameterAttribute(string name)
        {
            this.name = name;
        }
        
        public InjectParameterAttribute()
        {
            this.name = null;
        }
    }

    public abstract class AbstractParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public abstract IParameter Create();
        public virtual void DoPostProcess(IParameter parameter, IParameterContainer container) { }

        public AbstractParameterAttribute(string name)
        {
            Name = name;
        }

        public IParameter TryGet(IParameterContainer container, string name)
        {
            var p = container.Get(name);
            if (p == null) throw new Exception(string.Format("[{0}] Cannot find parameter {1} from {2}", GetType(), name, container));

            return p;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParameterAttribute : AbstractParameterAttribute
    {
        public ParameterAttribute(string name, float value = 0) : base(name)
        {
            this.Value = value;
        }

        public override IParameter Create() => new Parameter(Name, Value);
        
        public SecuredDouble Value { get; set; } = 0;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ConstantParameterAttribute : ParameterAttribute
    {
        public ConstantParameterAttribute(string name, float value = 0) : base(name, value)
        {

        }

        public override IParameter Create() => new ConstantParameter(Name, Value);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class BridgeParameterAttribute : AbstractParameterAttribute
    {
        private readonly double defaultValue;
        
        public BridgeParameterAttribute(string name) : base(name) { }

        public BridgeParameterAttribute(string name, string source) : base(name)
        {
            this.Source = source;
        }
        
        public BridgeParameterAttribute(string name, string source, double defaultValue) : base(name)
        {
            Source = source;
            this.defaultValue = defaultValue;
        }

        public override IParameter Create() => new BridgeParameter(Name, null, defaultValue);

        public override void DoPostProcess(IParameter parameter, IParameterContainer container)
        { 
            if (Source != null) (parameter as BridgeParameter).Source = TryGet(container, Source);
        }

        public string Source { get; set; }
        
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ClampedParameterAttribute : BridgeParameterAttribute
    {
        public ClampedParameterAttribute(string name, string source, double min, double max) : base(name, source)
        {
            Min = min;
            Max = max;
        }

        public SecuredDouble Min { get; set; }
        public SecuredDouble Max { get; set; }

        public override IParameter Create() => new ClampedParameter(Name, null, Min, Max);

    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class BoundParameterAttribute : AbstractParameterAttribute
    {
        public BoundParameterAttribute(string name, float minValue, float maxValue) : base(name)
        {
            this.MinValue = minValue;
            this.MaxValue = Mathf.Max(minValue, maxValue);
        }

        public override IParameter Create() => new BoundParameter(Name, MinValue, MaxValue);

        public SecuredDouble MinValue { get; }

        public SecuredDouble MaxValue { get; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class BaseEvaluatorAttribute : AbstractParameterAttribute
    {
        protected string[] parameters;

        public BaseEvaluatorAttribute(string name, params string[] parameters) : base(name)
        {
            this.parameters = new string[parameters.Length];
            Array.Copy(parameters, this.parameters, parameters.Length);
        }

        public override void DoPostProcess(IParameter parameter, IParameterContainer container)
        {
            var e = parameter as AbstractEvaluator;
            foreach (var i in Parameters)
            {
                e.Add(TryGet(container, i));
            }
        }

        public string[] Parameters => parameters;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SumAttribute : BaseEvaluatorAttribute
    {
        public SumAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new SumEvaluator(Name);
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AverageAttribute : BaseEvaluatorAttribute
    {
        public AverageAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new AverageEvaluator(Name);
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NonZeroAverageAttribute : BaseEvaluatorAttribute
    {
        public NonZeroAverageAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new NonZeroAverageEvaluator(Name);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ProductAttribute : BaseEvaluatorAttribute
    {
        public ProductAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new ProductEvaluator(Name);
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FirstNonZeroAttribute : BaseEvaluatorAttribute
    {
        public FirstNonZeroAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new FirstNonZeroEvaluator(Name);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AdaptProductAttribute : BaseEvaluatorAttribute
    {
        public AdaptProductAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new AdaptProductEvaluator(Name);
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SumStackAttribute : BaseEvaluatorAttribute
    {
        public SumStackAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new SumStackEvaluator(Name);
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SumInversedAttribute : BaseEvaluatorAttribute
    {
        public SumInversedAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public override IParameter Create() => new SumInversedEvaluator(Name);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class OneSumAttribute : BaseEvaluatorAttribute
    {
        private float value = 1;
        public OneSumAttribute(string name, params string[] parameters) : base(name, parameters) { }
        public OneSumAttribute(string name, float value, params string[] parameters) : base(name, parameters)
        {
            this.value = value;
        }

        public override IParameter Create()
        {
            var e = new OneSumEvaluator(Name);
            e.ConstValue = Value;
            return e;
        }

        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MinAttribute : BaseEvaluatorAttribute
    {
        public double DefaultValue = Double.MaxValue;
        public MinAttribute(string name, params string[] parameters) : base(name, parameters) { }
        public MinAttribute(string name, double defaultValue, params string[] parameters) : base(name, parameters)
        {
            DefaultValue = defaultValue;
        }

        public override IParameter Create() => new MinEvaluator(Name, DefaultValue);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MaxAttribute : BaseEvaluatorAttribute
    {
        public double DefaultValue = Double.MinValue;
        public MaxAttribute(string name, params string[] parameters) : base(name, parameters) { }

        public MaxAttribute(string name, double defaultValue, params string[] parameters) : base(name,
            parameters)
        {
            DefaultValue = defaultValue;
        }

        public override IParameter Create() => new MaxEvaluator(Name, DefaultValue);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class TwoParamsAttribute : AbstractParameterAttribute
    {
        public string A { get; set; }
        public string B { get; set; }

        protected TwoParamsAttribute(string name, string a, string b) : base(name)
        {
            A = a;
            B = b;
        }

        public override void DoPostProcess(IParameter parameter, IParameterContainer container)
        {
            (parameter as TwoParamsEvaluator).A = TryGet(container, A);
            (parameter as TwoParamsEvaluator).B = TryGet(container, B);
        }

    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DivAttribute : TwoParamsAttribute
    {
        public DivAttribute(string name, string a, string b) : base(name, a, b) { }

        public override IParameter Create() => new DivEvaluator(Name, null, null);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SubtractAttribute : TwoParamsAttribute
    {
        public SubtractAttribute(string name, string a, string b) : base(name, a, b) { }

        public override IParameter Create() => new SubtractEvaluator(Name, null, null);
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class PowAttribute : TwoParamsAttribute
    {
        public PowAttribute(string name, string a, string b) : base(name, a, b) { }

        public override IParameter Create() => new PowEvaluator(Name, null, null);
    }

    public class TernaryAttribute : AbstractParameterAttribute
    {
        private string a, b, condition;
        public TernaryAttribute(string name, string condition, string a, string b) : base(name)
        {
            this.condition = condition;
            this.a = a;
            this.b = b;
        }

        public override IParameter Create() => new TernaryEvaluator(Name);
        public override void DoPostProcess(IParameter parameter, IParameterContainer container)
        {
            if (!(parameter is TernaryEvaluator ternary))
            {
                Logger.Error<TernaryAttribute>($"{parameter} is not of type {typeof(TernaryEvaluator)}");
                return;
            }
            
            ternary.Condition = TryGet(container, condition);
            ternary.A = TryGet(container, a);
            ternary.B = TryGet(container, b);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DocAttribute : Attribute
    {
        public DocAttribute(string name)
        {

        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NoDoc : Attribute
    {
    }


    public class AutoAssignAttribute : Attribute
    {
        public string name;
        public AutoAssignAttribute(string name)
        {
            this.name = name;
        }
    }
    
    public class AutoBindAttribute : Attribute
    {
        public AutoBindAttribute(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
            set;
        }
    }

    public class CodeGen : Attribute
    {
        public CodeGen(bool isParameter = false)
        {
            IsParameter = isParameter;
        }

        public bool IsParameter
        {
            get;
            set;
        }
    }

}