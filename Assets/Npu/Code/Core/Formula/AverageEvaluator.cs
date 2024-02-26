using Npu.Core;

namespace Npu.Formula
{
    public class AverageEvaluator : AbstractEvaluator
    {
        public AverageEvaluator(string name) : base(name)
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            if (parameters.Count == 0) return 0;
           
            value = 0;
            for (var i = 0; i < parameters.Count; i++)
            {
                value += parameters[i].Value;
            }
            value /= parameters.Count;
            dirty = false;

            return value;
        }
    }
    
    public class NonZeroAverageEvaluator : AbstractEvaluator
    {
        public NonZeroAverageEvaluator(string name) : base(name)
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            if (parameters.Count == 0) return 0;
           
            value = 0;
            var c = 0;
            for (var i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Value != 0)
                {
                    c++;
                    value += parameters[i].Value;
                }
            }
            if (c != 0) value /= c;
            dirty = false;

            return value;
        }
    }
}