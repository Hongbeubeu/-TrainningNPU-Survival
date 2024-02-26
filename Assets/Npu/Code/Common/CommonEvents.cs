using UnityEngine.Events;
using System;

namespace Npu.Common
{

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }

    [Serializable]
    public class DoubleEvent : UnityEvent<double>
    {
    }

    [Serializable]
    public class LongEvent : UnityEvent<long>
    {
    }

}