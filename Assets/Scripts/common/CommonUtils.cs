using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommonUtils : MonoSingleton<CommonUtils>
{
    static public void updateImage(string imageResourcePath, Image uiImage)
    {
        if(imageResourcePath == null)
        {
            return;
        }
        Sprite image = Resources.Load<Sprite>(imageResourcePath);
        if(uiImage != null)
        {
            if (image != null)
            {
                uiImage.sprite = image;
                uiImage.SetNativeSize();
                uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, 255f);
            }
            else
            {
                uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, 0f);
            }
        }

    }

    static public T CreateViewByType<T>(GameObject prefab, Transform parent) where T : Component
    {
        // ʵ����prefab
        GameObject obj = Instantiate(prefab, parent);
        if (obj != null)
        {
            T view = obj.GetComponent<T>();
            if (!view)
            {
                // ����������T
                view = obj.AddComponent<T>();
            }
            return view;
        }
        // ���prefab��null����û�гɹ�ʵ����������Ĭ��ֵ
        return default(T);
    }

    static public Transform findChildByName(Transform parentTransform, string childName)
    {
        int childCount = parentTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                // �ݹ������ӽڵ���ӽڵ�
                Transform result = findChildByName(child, childName);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null; // ���û���ҵ�����ָ�����Ƶ��ӽڵ�
    }

    static public void updateText(string txt,Text textCom)
    {
        if (!string.IsNullOrEmpty(txt) && textCom!=null)
        {
            textCom.text = txt;
        }
        else
        {
            textCom.text = "";
        }
    }
}
