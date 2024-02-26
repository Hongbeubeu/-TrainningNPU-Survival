using Npu.Core;

namespace Npu.Formula
{
    public class OneSumEvaluator : SumEvaluator
    {
        private IParameter oneParam = new Parameter("Constant", 1);

        public SecuredDouble ConstValue
        {
            get { return oneParam.Value; }
            set
            {
                oneParam.Value = value;
            }
        }

        public OneSumEvaluator(string name) : base(name)
        {
            Add(oneParam);
        }

        public OneSumEvaluator()
        {
            Add(oneParam);
        }

        public OneSumEvaluator(SecuredDouble value)
        {
            oneParam.Value = value;
            Add(oneParam);
        }
    }

}