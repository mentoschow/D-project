using System;
using UnityEngine;
using UnityEngine.EventSystems; // �����¼�ϵͳ�������ռ�

public class ButtonClickPosition : MonoBehaviour, IPointerClickHandler
{
    Action<PointerEventData> onPointerClick;
    // ����ť�����ʱ���ô˺���
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