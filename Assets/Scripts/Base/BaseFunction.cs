using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
            obj.transform.SetParent(parent, false);
            var view = obj.GetComponent<T>();
            return view;
        }
        return default(T);
    }

    public static T CreateView<T>(GameObject prefab)
    {
        var obj = Instantiate(prefab);
        if (obj != null)
        {
            var view = obj.GetComponent<T>();
            return view;
        }
        return default(T);
    }

    public static string StripLength(Text text, int width)
    {
        int totalLength = 0;
        Font myFont = text.font;
        myFont.RequestCharactersInTexture(text.text, text.fontSize, text.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();

        char[] charArr = text.text.ToCharArray();

        int i = 0;
        for (; i < charArr.Length; i++)
        {
            myFont.GetCharacterInfo(charArr[i], out characterInfo, text.fontSize);

            int newLength = totalLength + characterInfo.advance;
            if (newLength > width)
            {
                if (Mathf.Abs(newLength - width) > Mathf.Abs(width - totalLength))
                {
                    break;
                }
                else
                {
                    totalLength = newLength;
                    break;
                }
            }
            totalLength += characterInfo.advance;
        }
        return text.text.Substring(0, i);
    }

    public static T ChangeStringToEnum<T>(string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }

    public static string FixStringChangeLine(string str)
    {
        string result = "";
        int startIndex = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '\\' && str[i + 1] == 'n')
            {
                result += str.Substring(startIndex, i - startIndex);
                result += "\n";
                startIndex = i + 2;
            }
        }
        result += str.Substring(startIndex, str.Length - startIndex);
        return result;
    }
}
