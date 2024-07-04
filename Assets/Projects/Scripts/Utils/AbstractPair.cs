using System;
using UnityEngine;

namespace Projects.Utils
{
    /// <summary>
    /// Pair
    /// </summary>
    [Serializable]
    public abstract class AbstractPair<TKey, TValue>
    {
        [SerializeField] private TKey _key;
        [SerializeField] private TValue _value;

        public TKey Key => _key;
        public TValue Value => _value;

        protected AbstractPair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
    }
}