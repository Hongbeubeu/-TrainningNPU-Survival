using System.Linq;
using Npu.Core;

namespace Npu.Formula
{
    public class MinEvaluator : AbstractEvaluator
    {
        private SecuredDouble defaultValue = double.MaxValue;
        public MinEvaluator(string name) : base(name) { }

        public MinEvaluator(string name, SecuredDouble defaultValue) : base(name)
        {
            this.defaultValue = defaultValue;
        }

        public MinEvaluator() : base("") { }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;

            if (parameters.Count == 0)
            {
                value = defaultValue;
            }
            else
            {
                value = double.MaxValue;
                for (var i = 0; i < parameters.Count; i++)
                {
                    value = value > parameters[i].Value ? parameters[i].Value : value;
                }
                
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "MIN(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }
    
    public class MaxEvaluator : AbstractEvaluator
    {
        private SecuredDouble defaultValue = double.MinValue;
        
        public MaxEvaluator(string name) : base(name) { }

        public MaxEvaluator(string name, SecuredDouble defaultValue) : base(name)
        {
            this.defaultValue = defaultValue;
        }

        public MaxEvaluator() : base("") { }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;

            if (parameters.Count == 0)
            {
                value = defaultValue;
            }
            else
            {
                value = double.MinValue;
                for (var i = 0; i < parameters.Count; i++)
                {
                    value = value < parameters[i].Value ? parameters[i].Value : value;
                }
                
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "MAX(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }
}