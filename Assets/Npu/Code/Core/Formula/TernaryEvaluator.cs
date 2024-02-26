using System;
using Npu.Core;

namespace Npu.Formula
{
    public class TernaryEvaluator : IParameter
    {
        public event Action<IParameter> Changed;
        public string Name { get; set; }

        public TernaryEvaluator(string name, IParameter condition, IParameter a, IParameter b)
        {
            Name = name;
            Condition = condition;
            A = a;
            B = b;
        }
        
        public TernaryEvaluator(string name)
        {
            Name = name;
        }
        
        public SecuredDouble Value
        {
            get => condition.Value.AsBool(1e-5f) ? a.Value : b.Value;
            set => Logger.Error<TernaryEvaluator>($"{Name}: setter is not allowed");
        }


        private IParameter condition;
        public IParameter Condition
        {
            get => condition;
            set
            {
                if (condition != null) condition.Changed -= OnSourceChanged;
                condition = value;
                if (condition != null) condition.Changed += OnSourceChanged;
            }
        }
        
        private IParameter a;
        public IParameter A
        {
            get => a;
            set
            {
                if (a != null) a.Changed -= OnSourceChanged;
                a = value;
                if (a != null) a.Changed += OnSourceChanged;
            }
        }
        
        private IParameter b;
        public IParameter B
        {
            get => b;
            set
            {
                if (b != null) b.Changed -= OnSourceChanged;
                b = value;
                if (b != null) b.Changed += OnSourceChanged;
            }
        }
        
        public string Description => $"{Name} ({GetType()}, Condition = {condition?.Name}, A={a?.Name}, B={b?.Name})";
        
        private void OnSourceChanged(IParameter parameter)
        {
            Changed?.Invoke(this);
        }
    }
}