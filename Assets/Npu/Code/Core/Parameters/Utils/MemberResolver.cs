using System;

namespace Npu.Exprimental
{
    [Serializable]
    public abstract class MemberResolver
    {
        public Type Type { get; private set; }
        public string Member { get; private set; }

        public MemberResolver(Type type)
        {
            Type = type;
        }

        public abstract object Resolve(object target);
        public abstract string[] Members { get; }

    }
}