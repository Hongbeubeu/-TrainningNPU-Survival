using System.Linq;
using Npu.Core;

namespace Npu.Formula
{

    public class ProductEvaluator : AbstractEvaluator
    {
        public ProductEvaluator(string name) : base(name) { }
        public ProductEvaluator() : base("") { }

        public override SecuredDouble Evaluate()
        {
            if (!dirty) return value;
            
            value = 1;
            for (var i = 0; i < parameters.Count; i++)
            {
                value *= parameters[i].Value;
            }
            dirty = false;

            return value;
        }

        public override string LongDescription => "Product(" + string.Join(",", parameters.Select(i => i.Description).ToArray()) + ")";
    }

}