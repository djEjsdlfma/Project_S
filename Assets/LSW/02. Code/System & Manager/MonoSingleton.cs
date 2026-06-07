using System;
using UnityEngine;

namespace LSW._02._Code.System___Manager
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            try
            {
                if (_instance != null)
                {
                    if (_instance != this)
                    {
                        Destroy(_instance.gameObject);
                        _instance = this as T;
                        return;
                    }
                }
                else
                {
                    _instance = this as T;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected MonoSingleton() { }

        protected MonoSingleton(bool shouldCreateNewInstance)
        {
            if (shouldCreateNewInstance)
            {
                _instance = null;
            }
        }
    }
}