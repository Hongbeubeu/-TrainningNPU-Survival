namespace Npu.Common
{
    public interface IKeyedValueProvider
    {
        string[] Keys { get; }
        object Get(string key);
    }
}