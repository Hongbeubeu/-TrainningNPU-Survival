using System;

namespace Npu
{
    public interface IDataProvider
    {
        event Action<IDataProvider, int> DataChanged;
        bool Ready { get; }
        Type GetDataType();
        object GetData();
        string[] BindingFilters { get; }
    }

    /**

#region IDataProvider
    public event System.Action<IDataProvider, int> DataChanged;
    public bool Ready => true;
    public System.Type GetDataType() => GetType();
    public object GetData() => this;
    private static string[] _bindingFilters = {"default"};
    public string[] BindingFilters => _bindingFilters;
    
    protected void TriggerChanged(int mask) => DataChanged?.Invoke(this, mask);
    protected void TriggerChanged(string mask) => DataChanged?.Invoke(this, this.GetFilters(mask));
    
    [ContextMenu("Trigger Change All")]
    private void TriggerChangedAll() => TriggerChanged(~0);
#endregion    

**/

    public interface IDataFormatter
    {
        bool Format(object data, out object formatted);
    }

    public static class DataProviderExtension
    {
        public static int GetFilters(this IDataProvider provider, params string[] filters)
        {
            var value = 0x00;
            int index;
            for (var i = 0; i < filters.Length; i++)
            {
                if ((index = System.Array.IndexOf(provider.BindingFilters, filters[i])) >= 0)
                {
                    value |= 1 << index;
                }
            }

            return value;
        }
    }
    
}