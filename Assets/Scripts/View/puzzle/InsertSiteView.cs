using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InsertSiteView : MonoSingleton<InsertSiteView>
{
    public Image sitePic;
    public Image voidPic;

    static GameObject insertViewPrefab;
    public static GameObject getPrefab()
    {
        if (!InsertSiteView.insertViewPrefab)
        {
            string resourceName = "Prefabs/UI/puzzle/InsertSiteView"; // ×ÊÔ´Ãû³Æ
            InsertSiteView.insertViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return InsertSiteView.insertViewPrefab;
    }
    private void Awake()
    {
        //Debug.Log("awake");
        sitePic = transform.Find("sitePic")?.GetComponent<Image>();
        voidPic = transform.Find("voidPic")?.GetComponent<Image>();
    }
    public void updateView(string url)
    {
        bool hasInsert = url != null;
        sitePic.gameObject.SetActive(hasInsert);
        voidPic.gameObject.SetActive(!hasInsert);
        if (hasInsert)
        {
            CommonUtils.updateImage(url,sitePic);
        }
    }
}
