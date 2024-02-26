using Npu.Core;
using UnityEngine;

namespace Npu.Formula
{
    public abstract class AbstractFormula : ScriptableObject
    {
        public abstract SecuredDouble Evaluate(int n);
        
        /// <summary>
        /// Gets aggregated value 
        /// </summary>
        public virtual SecuredDouble AggregatedValue(int n0, int count)
        {
            SecuredDouble cost = 0;
            for (var i = 0; i < count; i++) cost += Evaluate(n0 + i);
            return cost;
        }
    }
}