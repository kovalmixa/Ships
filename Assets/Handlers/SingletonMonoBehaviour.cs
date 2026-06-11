using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lock_ = new object();
    private static bool isQuitting = false;

    public static T Instance
    {
        get
        {
            if (isQuitting) return null;
            lock (lock_)
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).Name + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
