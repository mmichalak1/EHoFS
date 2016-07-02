using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OurGame.Engine.Utilities
{
    [Serializable]
    public class DictionaryItem<T,V>
    {
        public DictionaryItem()
        {

        }
        public DictionaryItem(T key, V value)
        {
            Key = key;
            Value = value;
        }
        public T Key;
        public V Value;
    }
}
