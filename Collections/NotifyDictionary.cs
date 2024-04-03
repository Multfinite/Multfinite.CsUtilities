using System.Collections;
using System.Collections.Specialized;
using ACTION = System.Collections.Specialized.NotifyCollectionChangedAction;
using NCCEA = System.Collections.Specialized.NotifyCollectionChangedEventArgs;

namespace Multfinite.Utilities.Collections
{
	public class NotifyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private IDictionary<TKey, TValue> _dictionary;
        protected IDictionary<TKey, TValue> Dictionary => _dictionary;

        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set
            {
                Dictionary[key] = value; 
                ValueChanged?.Invoke(this, key, value);
                OnCollectionChanged(new NCCEA(ACTION.Add, new List<KeyValuePair<TKey, TValue>>()
                { new KeyValuePair<TKey, TValue>(key, value) }));
            }
        }
        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TValue> Values => Dictionary.Values;
        public int Count => Dictionary.Count;
        public bool IsReadOnly => Dictionary.IsReadOnly;

        public event KeyObjectDelegate<TKey, TValue> ValueChanged;
        public event KeyObjectDelegate<TKey, TValue> ValueAdded;
        public event KeyObjectDelegate<TKey, TValue> ValueRemoved;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(NCCEA args)
        {
            CollectionChanged?.Invoke(this, args);
        }
        private List<KeyValuePair<TKey, TValue>> Items()
        {
            List<KeyValuePair<TKey, TValue>> oldItems = new List<KeyValuePair<TKey, TValue>>();
            foreach (var kvp in Dictionary) oldItems.Add(kvp);
            return oldItems;
        }

        public NotifyDictionary() => _dictionary = new Dictionary<TKey, TValue>();
        public NotifyDictionary(IDictionary<TKey, TValue> sourceDictionary) => _dictionary = new Dictionary<TKey, TValue>(sourceDictionary);
        public NotifyDictionary(int capacity) => _dictionary = new Dictionary<TKey, TValue>(capacity);
        public NotifyDictionary(IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(comparer);
        public NotifyDictionary(IDictionary<TKey, TValue> sourceDictionary, IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(sourceDictionary, comparer);

        public void Add(TKey key, TValue value) => Add(new KeyValuePair<TKey, TValue>(key, value));
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            //List<KeyValuePair<TKey, TValue>> oldItems = Items();
            Dictionary.Add(item);
            ValueAdded?.Invoke(this, item.Key, item.Value);
            OnCollectionChanged(new NCCEA(ACTION.Add, item));
        }
        public void Clear()
        {
            foreach (var kvp in Dictionary) {ValueRemoved?.Invoke(this, kvp.Key, kvp.Value);}
            OnCollectionChanged(new NCCEA(ACTION.Reset));
            Dictionary.Clear();
        }
        public bool Remove(TKey key)
        {
            TKey _key = key;
            TValue _value = Dictionary[_key];
            bool result = Dictionary.Remove(key);
            if (result)
            {
                OnCollectionChanged(new NCCEA(ACTION.Remove, new List<KeyValuePair<TKey, TValue>>()
                { new KeyValuePair<TKey, TValue>(_key, _value) }));
                ValueRemoved?.Invoke(this, _key, _value);
            }
            return result;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TKey _key = item.Key;
            TValue _value = item.Value;
            bool result = Dictionary.Remove(item);
            if (result)
            {
                OnCollectionChanged(new NCCEA(ACTION.Remove, new List<KeyValuePair<TKey, TValue>>()
                { item }));
                ValueRemoved?.Invoke(this, _key, _value);
            }
            return result;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Contains(item);
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { Dictionary.CopyTo(array, arrayIndex); }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return Dictionary.GetEnumerator(); }
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
    }
}