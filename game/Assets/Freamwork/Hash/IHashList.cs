using System.Collections.Generic;

namespace Freamwork
{
    public interface IHashList<K, V>
    {
        int count { get; }
        List<K> keys { get; }
        List<V> values { get; }
        V getValue(K key);
        V getValueAt(int index);
        void add(K key, V value);
        bool insert(int index, K key, V value);
        V remove(K key);
        V removeAt(int index);
        bool containsKey(K key);
        bool containsValue(V value);
        void clear();
    }
}
