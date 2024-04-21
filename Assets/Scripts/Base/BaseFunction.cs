using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BaseFunction : MonoBehaviour
{
    public static Sprite LoadImage(string imgUrl)
    {
        var imageData = File.ReadAllBytes(imgUrl);
        var texture = new Texture2D(8, 8);
        texture.LoadImage(imageData);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public static T CreateView<T>(GameObject prefab, Transform parent)
    {
        var obj = Instantiate(prefab);
        if (obj != null)
        {
            obj.transform.SetParent(parent);
            var view = obj.GetComponent<T>();
            return view;
        }
        return default(T);
    }
}
