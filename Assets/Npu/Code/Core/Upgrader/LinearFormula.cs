using Npu.Core;
using UnityEngine;

namespace Npu.Formula
{
    [CreateAssetMenu(fileName = "LinearFormula", menuName = "Formula/LinearFormula", order = 0)]
    public class LinearFormula : AbstractFormula
    {
        [SerializeField, FormulaDrawer(typeof(LinearFormula), "f(n)", "[F]a + [F]b * n", false, 1, 100)] private SecuredDouble a = 1;
        [SerializeField, HideInInspector] private SecuredDouble b = 1;

        public override SecuredDouble Evaluate(int n) => a + b * n;

    }
}