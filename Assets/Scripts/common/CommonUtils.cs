using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CommonUtils
{
    static public void updateImage(string imageResourcePath, Image uiImage)
    {
        if(imageResourcePath == null)
        {
            return;
        }
        Sprite image = Resources.Load<Sprite>(imageResourcePath);
        if (image != null && uiImage != null)
        {
            uiImage.sprite = image;
        }

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
                // 递归搜索子节点的子节点
                Transform result = findChildByName(child, childName);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null; // 如果没有找到具有指定名称的子节点
    }
}
