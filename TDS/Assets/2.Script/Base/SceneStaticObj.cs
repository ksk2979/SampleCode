using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬 안에서만 사용하는 참조 스크립트 / 씬을 나가면 없어지는 스크립트
public class SceneStaticObj<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = default(T);

    public static T GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
            if (_instance == null)
            {
                var gameObject = new GameObject(typeof(T).ToString());
                _instance = gameObject.AddComponent<T>();
            }

            return _instance;
        }
    }
}
