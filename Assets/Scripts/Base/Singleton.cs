using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitable
{
    void Init();
}
public class Singleton<T> where T : new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                (_instance as IInitable)?.Init();
            }

            return _instance;
        }
    }
}
