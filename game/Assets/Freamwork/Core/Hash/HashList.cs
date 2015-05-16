using System.Collections.Generic;

namespace Freamwork
{
    public class HashList<K, V> : IHashList<K, V>
    {
        private List<K> keyList;
        private Dictionary<K, V> dic;

        public HashList()
        {
            keyList = new List<K>();
            dic = new Dictionary<K, V>();
        }

        public int count
        {
            get
            {
                return keyList.Count;
            }
        }

        public List<K> keys
        {
            get
            {
                return new List<K>(keyList);
            }
        }

        public List<V> values
        {
            get
            {
                List<V> list = new List<V>();
                for (int i = 0; i < keyList.Count; i++)
                {
                    list.Add(dic[keyList[i]]);
                }
                return list;
            }
        }

        public V getValue(K key)
        {
            return dic[key];
        }

        public V getValueAt(int index)
        {
            return dic[keyList[index]];
        }

        public void add(K key, V value)
        {
            keyList.Add(key);
            dic.Add(key, value);
        }

        public bool insert(int index, K key, V value)
        {
            if (index >= 0 && index <= keyList.Count)
            {
                keyList.Insert(index, key);
                dic.Add(key, value);
                return true;
            }
            return false;
        }

        public V remove(K key)
        {
            V value = default(V);
            if (keyList.Contains(key))
            {
                keyList.Remove(key);
                value = dic[key];
                dic.Remove(key);
                return value;
            }
            return value;
        }

        public V removeAt(int index)
        {
            V value = default(V);
            if (index >= 0 && index < keyList.Count)
            {
                value = getValueAt(index);
                dic.Remove(keyList[index]);
                keyList.RemoveAt(index);
            }
            return value;
        }

        public bool containsKey(K key)
        {
            return keyList.Contains(key);
        }

        public bool containsValue(V value)
        {
            return dic.ContainsValue(value);
        }

        public void clear()
        {
            keyList.Clear();
            dic.Clear();
        }
    }
}
