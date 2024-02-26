using Npu.Core;
using UnityEngine;

namespace Npu.Formula
{
    [CreateAssetMenu(fileName = "ExponentialFormula", menuName = "Formula/ExponentialFormula", order = 0)]
    public class ExponentialFormula : AbstractFormula
    {
        [SerializeField, FormulaDrawer(typeof(ExponentialFormula), "f(n)", "[F]a + [F]b * [F]c ^ n", false, 1, 100)] private SecuredDouble a = 0;
        [HideInInspector, SerializeField] private SecuredDouble b = 1, c = 1.15;

        public override SecuredDouble Evaluate(int n) => a + b * c.Pow(n);
        public override SecuredDouble AggregatedValue(int n0, int count) => UpgradeUtils.GetBulkCost(b, c, n0, count) + count * a;
    }
}