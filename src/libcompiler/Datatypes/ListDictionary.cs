using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;

namespace libcompiler.Datatypes
{
    public class ListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        private Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();

        public List<TValue> this[TKey key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                _dictionary.Add(key, new List<TValue>{value});
            }
        }

        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            foreach (TValue value in values)
            {
                Add(key, value);
            }
        }

        public int Count => _dictionary.Count;


        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}