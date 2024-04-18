using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, new[] { typeof(T) });
                    DontDestroyOnLoad(obj);
                    _instance = obj.GetComponent<T>();
                    (_instance as IInitable)?.Init();
                }
                else
                {
                    Debug.LogWarning("Instance is already exist!");
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// �̳�Mono�����������д��Awake��������Ҫ��Awake�����ʼ�ĵط�����һ��base.Awake()������_instance��ֵ
    /// </summary>
    private void Awake()
    {
        _instance = this as T;
        DontDestroyOnLoad(this);
    }
}
