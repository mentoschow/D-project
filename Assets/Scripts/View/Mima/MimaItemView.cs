using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MimaItemView : MonoSingleton<MimaItemView>
{
    // Start is called before the first frame update
    Image sitePic;
    public int curIndex = 0;

    static GameObject ViewPrefab;
    public static GameObject getPrefab()
    {
        if (!MimaItemView.ViewPrefab)
        {
            string resourceName = "Prefabs/UI/Mima/MimaItemView"; // ��Դ����
            MimaItemView.ViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return MimaItemView.ViewPrefab;
    }

    public void init(int number,string urlName)
    {
        string url = "Images/UI/Mima/������ͼ/" + urlName;
        CommonUtils.updateImage(url, sitePic);
        this.curIndex = number;
    }

    private void Awake()
    {
      sitePic = CommonUtils.findChildByName(transform, "sitePic").GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
