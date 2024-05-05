using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragNodeCompent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    // Start is called before the first frame update
    Action<GameObject,int> dragStartFun;
    Action<Vector2> dragMoveFun;
    Action<Vector2, int> dragOverFun;
    Image pic;
    Vector3 originPos = Vector3.zero;

    int selfCode = 0;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    bool hasInit = false;
    public void init(
        Action<GameObject, int> dragStartCallback
        , Action<Vector2> dragMoveCallback
        , Action<Vector2, int> dragOverCallback
        , int code,Image pic)
    {
        if (this.hasInit)
        {
            return;
        }
        this.hasInit = true;
        this.dragStartFun = dragStartCallback;
        this.dragOverFun = dragOverCallback;
        this.dragMoveFun = dragMoveCallback;

        this.selfCode = code;
        this.pic = pic;
        this.originPos = pic.gameObject.transform.localPosition;
    }

    Action pointerEnterCallback;
    Action pointerExitCallback;
    public void initPointer(Action pointerEnterCallback,Action pointerExitCallback)
    {
       this.pointerEnterCallback = pointerEnterCallback;
        this.pointerExitCallback = pointerExitCallback;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(this.pointerEnterCallback != null)
        {
            this.pointerEnterCallback();
            // 当鼠标悬停在此UI元素上时调用
            Debug.Log("Mouse entered " + gameObject.name);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.pointerExitCallback!= null)
        {
            this.pointerExitCallback();
            // 当鼠标离开此UI元素时调用
            Debug.Log("Mouse exited " + gameObject.name);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //this.originPos = pic.gameObject.transform.localPosition;
        if (this.dragStartFun != null)
        {
            this.dragStartFun(transform.gameObject,this.selfCode);
            this.pic.color = new Color(this.pic.color.r, this.pic.color.g, this.pic.color.b, 0f);
        }
        Debug.Log("开始拖拽");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out pos);
        rectTransform.position = pos;

        if (this.dragMoveFun != null)
        {
            this.dragMoveFun(pos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(this.dragOverFun!= null)
        {
            Vector3 worldPosition3D = transform.position;
            Vector2 finalPos = new Vector2(worldPosition3D.x, worldPosition3D.y);
            this.dragOverFun(finalPos,selfCode);
            this.pic.color = new Color(this.pic.color.r, this.pic.color.g, this.pic.color.b, 255f);
            transform.localPosition = this.originPos;
        }
        Debug.Log("结束拖拽");
    }

}