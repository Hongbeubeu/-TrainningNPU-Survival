using System.Linq;
using Npu.Core;

namespace Npu.Formula
{

    public class SumEvaluator : AbstractEvaluator
    {
        public SumEvaluator(string name) : base(name) { }

        public SumEvaluator() : base("")
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
           
            value = 0;
            for (var i = 0; i < parameters.Count; i++)
            {
                value += parameters[i].Value;
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "SUM(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }

    public class AdaptProductEvaluator : AbstractEvaluator
    {
        public AdaptProductEvaluator(string name) : base(name) { }

        public AdaptProductEvaluator() : base("")
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            
            value = 1;
            for (var i = 0; i < parameters.Count; i++)
            {
                var v = parameters[i].Value;
                value += v > 1 ? v - 1 : 0;
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "SUM(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }
    
    public class SumStackEvaluator : AbstractEvaluator
    {
        public SumStackEvaluator(string name) : base(name) { }

        public SumStackEvaluator() : base("")
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            
            if (parameters.Count == 0)
            {
                value = 1;
            }
            else
            {
                value = 0;
                for (var i = 0; i < parameters.Count; i++)
                {
                    var v = parameters[i].Value;
                    value += v;
                }
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "SUM(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }
    
    public class SumInversedEvaluator : AbstractEvaluator
    {
        public SumInversedEvaluator(string name) : base(name) { }

        public SumInversedEvaluator() : base("")
        {
        }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            
            if (parameters.Count == 0)
            {
                value = 0;
            }
            else
            {
                value = 0;
                for (var i = 0; i < parameters.Count; i++)
                {
                    var v = parameters[i].Value;
                    value += 1/v;
                }
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "SUM_INVERSED(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }

}