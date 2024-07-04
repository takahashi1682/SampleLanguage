using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Projects.Utils
{
    /// <summary>
    /// SerializableDictionary
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue, TYpe> where TYpe : AbstractPair<TKey, TValue>
    {
        [SerializeField]
        private List<TYpe> _list;

        private Dictionary<TKey, TValue> _dictionary;
        public Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                _dictionary ??= ConvertToDictionary();
                return _dictionary;
            }
        }

        /// <summary>
        /// ConvertToDictionary
        /// </summary>
        private Dictionary<TKey, TValue> ConvertToDictionary()
        {
            return _list.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}