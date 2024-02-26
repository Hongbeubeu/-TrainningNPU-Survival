using System;
using Npu.Core;

namespace Npu.Formula
{

    public abstract class TwoParamsEvaluator : IParameter
    {
        public event Action<IParameter> Changed;

        protected TwoParamsEvaluator(string name, IParameter a, IParameter b)
        {
            Name = name;
            A = a;
            B = b;
        }

        protected TwoParamsEvaluator(string name, double a, double b)
        {
            Name = name;
            A = new Parameter("A", a);
            B = new Parameter("B", b);
        }

        protected TwoParamsEvaluator(string name) : this(name, 0, 0)
        {

        }

        protected IParameter _a;
        public IParameter A
        {
            get => _a;
            set
            {
                if (_a != null) _a.Changed -= OnSourceChanged;
                _a = value;
                if (_a != null) _a.Changed += OnSourceChanged;
            }
        }

        protected IParameter _b;
        public IParameter B
        {
            get => _b;
            set
            {
                if (_b != null) _b.Changed -= OnSourceChanged;
                _b = value;
                if (_b != null) _b.Changed += OnSourceChanged;
            }
        }

        public virtual SecuredDouble Value { get; set; }

        protected void OnSourceChanged(IParameter parameter)
        {
            Changed?.Invoke(this);
        }

        public string Name { get; set; }
        public virtual string Description => string.Format("{0} ({1}, A={2}, B={3})", Name, GetType(), _a?.Name, _b?.Name);
    }

    public class SubtractEvaluator : TwoParamsEvaluator
    {
        public override SecuredDouble Value => _a.Value - _b.Value;

        public SubtractEvaluator(string name, IParameter a, IParameter b) : base(name, a, b) { }
        public SubtractEvaluator(string name, double a, double b) : base(name, a, b) { }
    }


    public class DivEvaluator : TwoParamsEvaluator
    {
        public override SecuredDouble Value => _a.Value / _b.Value;

        public DivEvaluator(string name, IParameter a, IParameter b) : base(name, a, b) { }
        public DivEvaluator(string name, double a, double b) : base(name, a, b) { }
    }

    public class PowEvaluator : TwoParamsEvaluator
    {
        public override SecuredDouble Value => _a.Value.Pow(_b.Value);

        public PowEvaluator(string name, IParameter a, IParameter b) : base(name, a, b) { }
        public PowEvaluator(string name, double a, double b) : base(name, a, b) { }
    }

}