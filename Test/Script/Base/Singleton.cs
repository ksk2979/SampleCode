using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = default(T);
    private static readonly object _lock = new object();

    public static T GetInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            var singletonObject = new GameObject(typeof(T).ToString());
                            _instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
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
            DontDestroyOnLoad(gameObject);
            //Debug.Log($"Singleton Awake: {typeof(T)} Instance ID: {_instance.GetInstanceID()} initialized.");
        }
        else if (_instance != this)
        {
            //Debug.Log($"Singleton Awake: Duplicate instance of {typeof(T)} detected, destroying instance ID: {GetInstanceID()}.");
            Destroy(gameObject);
        }
    }

    public string InstanceCheck() { return $"Instance ID: {_instance.GetInstanceID()}"; }
}
