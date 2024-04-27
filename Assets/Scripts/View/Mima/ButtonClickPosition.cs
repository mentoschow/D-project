using System;
using UnityEngine;
using UnityEngine.EventSystems; // 引入事件系统的命名空间

public class ButtonClickPosition : MonoBehaviour, IPointerClickHandler
{
    Action<PointerEventData> onPointerClick;
    // 当按钮被点击时调用此函数
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData == null)
            return;
        if(this.onPointerClick != null) {
            this.onPointerClick(eventData);
        }
    }

    public void initClick(Action<PointerEventData> onPointerClick)
    {
        this.onPointerClick = onPointerClick;
    }
}