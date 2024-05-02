using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ClueCombineContainView : MonoSingleton<ClueCombineView>
{
    static GameObject ViewPrefab;
    ClueCombineItemView itemView;
    public static GameObject getPrefab()
    {
        if (!ClueCombineContainView.ViewPrefab)
        {
            string resourceName = "Prefabs/UI/ClueCombine/ClueCombineContainView"; // ×ÊÔ´Ãû³Æ
            ClueCombineContainView.ViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return ClueCombineContainView.ViewPrefab;
    }
    private void Awake()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void updateView(string containCode)
    {
        Destroy(itemView?.gameObject);
        itemView = null;

        if(!String.IsNullOrEmpty(containCode))
        {
            itemView = CommonUtils.CreateViewByType<ClueCombineItemView>(ClueCombineItemView.getPrefab(),transform);
            itemView.UpdateView(containCode);
        }
    }
}