using Npu.Core;

namespace Npu.Formula
{
    public class FirstNonZeroEvaluator : AbstractEvaluator
    {
        public FirstNonZeroEvaluator(string name) : base(name) { }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
           
            for (var i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Value != 0)
                {
                    value = parameters[i].Value;
                    break;
                }
            }
            dirty = false;

            return value;
        }
    }
}