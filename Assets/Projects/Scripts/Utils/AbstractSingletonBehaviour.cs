using System;
using UnityEngine;

namespace Projects
{
    /// <summary>
    ///     シングルトン基底クラス
    /// </summary>
    /// <typeparam name="T">MonoBehaviourを継承したクラス</typeparam>
    public abstract class AbstractSingletonBehaviour<T> : MonoBehaviour where T : AbstractSingletonBehaviour<T>
    {
        private static T _instance;
        protected abstract bool IsDontDestroyOnLoad { get; }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // シーン上からインスタンスを探す
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        throw new Exception($"{typeof(T).Name} オブジェクトが見つかりませんでした");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            if (this != _instance)
            {
                throw new Exception($"{nameof(T)} は既に存在します");
            }

            if (IsDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == _instance)
            {
                _instance = null;
            }
        }

        protected static void SetInstance(T value)
        {
            _instance = value;
        }
    }
}