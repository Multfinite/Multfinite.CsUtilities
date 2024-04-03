namespace Multfinite.Utilities.Collections
{
    public interface INotifyDictionary<TKey, TValue>
    {
        event KeyObjectDelegate<TKey, TValue> ValueChanged;
        event KeyObjectDelegate<TKey, TValue> ValueAdded;
        event KeyObjectDelegate<TKey, TValue> ValueRemoved;         
    }
}