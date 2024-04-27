using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MimaItemContainerView : MonoSingleton<MimaItemContainerView>
{
    // Start is called before the first frame update
    public GameObject mainObject;
    Action clickFunc;
    public int curNumber = 2;

    int rollNodeCount = 5;
    int middleNumber = 2;
    int changeDiff = 1;
    List<MimaItemView> rollNodeList = new List<MimaItemView>();

    ButtonClickPosition buttonCom;
    float moveTime = 1f;
    bool isMove = false;
    public bool isCanUse = true;
    private void Awake()
    {
        if(buttonCom == null)
        {
            buttonCom = gameObject.AddComponent<ButtonClickPosition>();
            buttonCom.initClick((PointerEventData eventData) =>
            {
                this.OnPointerClick(eventData);
            });
        }
        for(int i=0; i<rollNodeCount; i++)
        {
            MimaItemView mimaItemView = CommonUtils.CreateViewByType<MimaItemView>(MimaItemView.getPrefab(), transform);
            rollNodeList.Add(mimaItemView);
        }
    }

    public void init(int originNumber,Action clickFunc)
    {
        this.clickFunc = clickFunc;
        this.curNumber = originNumber;

        for(int i=0;i<rollNodeList.Count;i++)
        {
            MimaItemView mimaItemView = rollNodeList[i];
            mimaItemView.transform.localPosition = new Vector2(mimaItemView.transform.localPosition.x, getPosFromIndex(i));
            mimaItemView.init(i);
        }
        isMove = false;
    }

    public float getPosFromIndex(int index)
    {
        float result = 0;
        int diff = index-middleNumber ;


        float height = 0f;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 rectSize = rectTransform.rect.size;
            height = rectSize.y; // 获取UI元素的高度
        }

        result = height * diff;

        return result;
    }

    //public int getNewIndex(int targetIndex)
    //{
    //    int resultIndex = targetIndex;
    //}
    public void checkToRoll(bool bottom)
    {
        isMove = true;
        int addIndex = bottom ?-changeDiff : changeDiff;

        List<MimaItemView> popUpitems = new List<MimaItemView>();
        List<MimaItemView> normalItems = new List<MimaItemView>();
        for (int i = 0; i < rollNodeList.Count; i++)
        {
            MimaItemView mimaItemView = rollNodeList[i];
            int targetIndex = i + addIndex;
            if (targetIndex < 0)
            {
                popUpitems.Add(mimaItemView);
            }else if (targetIndex >= rollNodeList.Count)
            {
                popUpitems.Insert(0,mimaItemView);
            }
            else
            {
                normalItems.Add(mimaItemView);
            }
        }
        this.rollNodeList = new List<MimaItemView>();
        if (addIndex>0)
        {
            foreach (MimaItemView item in popUpitems)
            {
                this.rollNodeList.Add(item);
            }
            foreach (MimaItemView item in normalItems)
            {
                this.rollNodeList.Add(item);
            }
        }
        else
        {
            foreach (MimaItemView item in normalItems)
            {
                this.rollNodeList.Add(item);
            }
            foreach (MimaItemView item in popUpitems)
            {
                this.rollNodeList.Add(item);
            }
        }
        for (int i = 0; i < rollNodeList.Count; i++)
        {
            MimaItemView mimaItemView = rollNodeList[i];

            int targetIndex = i;
            float posY = getPosFromIndex(targetIndex);
            if(targetIndex == middleNumber)
            {
                curNumber = mimaItemView.curIndex;
            }
            Vector3 pos = new Vector3(mimaItemView.transform.localPosition.x, posY, 0);
            if (targetIndex == 0 || targetIndex == rollNodeList.Count - 1)
                {
                    mimaItemView.transform.localPosition = pos;
                }
                else
                {
                Debug.Log("点击位置在按钮上: "+ mimaItemView.transform.localPosition.y+","+pos.y);
                mimaItemView.transform.DOLocalMove(pos, changeDiff * moveTime);
                }
        }
        // 创建一个延时的 Tween
        Tween delayTween = DOTween.Sequence()
            .AppendInterval(changeDiff * moveTime)
            .OnComplete(() => {
                isMove = false;
                if(this.clickFunc != null)
                {
                    this.clickFunc();
                }
            });
        // 激活延时 Tween
        delayTween.Play();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData == null || isMove || !isCanUse)
        {
            return;
        }

        // 获取点击位置在世界空间中的坐标
        Vector3 clickPosition = eventData.position;

        // 转换点击位置到Canvas的本地坐标空间
        Vector2 localClickPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            this.GetComponent<RectTransform>(), clickPosition, eventData.pressEventCamera, out localClickPosition))
        {
            // localClickPosition现在包含了点击位置在按钮上的本地坐标
            if (localClickPosition.y != 0)
            {
                this.checkToRoll(localClickPosition.y < 0);
            }
            // 在这里处理你的逻辑，例如打印点击位置
            Debug.Log("点击位置在按钮上: " + localClickPosition);
        }
    }
}
