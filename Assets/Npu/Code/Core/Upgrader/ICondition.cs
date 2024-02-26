using System;

namespace Npu.Core
{
    public interface ICondition
    {
        bool Meets { get; }
        // bool RequiredNewFactory { get; }
        event Action<ICondition> Changed;
        string Description { get; }
        void Setup();
        void TearDown();
    }

}