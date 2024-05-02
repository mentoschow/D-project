using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ClueCombineItemView : MonoBehaviour
{
    public Sprite bg_boy_normal;
    //public Sprite bg_boy_click;
    public Sprite bg_girl_normal;
    //public Sprite bg_girl_click;

    [SerializeField]
    private Text text;
    [SerializeField]
    private float aniTime = 0.2f;
    [SerializeField]
    private float stayTime = 2f;
    [SerializeField]
    private Image img;

    private RectTransform rect;
    private string itemID;

    DragNodeCompent dragCom;

    static GameObject ViewPrefab;
    public static GameObject getPrefab()
    {
        if (!ClueCombineItemView.ViewPrefab)
        {
            string resourceName = "Prefabs/UI/ClueCombine/ClueCombineItemView"; // ×ÊÔ´Ãû³Æ
            ClueCombineItemView.ViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return ClueCombineItemView.ViewPrefab;
    }
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void refreshContainState(bool isActive)
    {
        this.img.gameObject.SetActive(isActive);

        if (this.dragCom)
        {
            this.dragCom.enabled = isActive;
        }
    }

    public void updateDrag(Action<GameObject, int> dragStartCallback
        , Action<Vector2> dragMoveCallback
        , Action<Vector2, int> dragOverCallback)
    {
        if (!this.dragCom)
        {
            this.dragCom = gameObject.AddComponent<DragNodeCompent>();
            this.dragCom.enabled = false;
        }
        this.dragCom?.init(dragStartCallback, dragMoveCallback, dragOverCallback, 1, this.img);
    }

    public void UpdateView(string itemID)
    {
        var config = ConfigController.Instance.GetClueItemConfig(itemID);
        if (config != null)
        {
            var curRoleType = RoleController.Instance.curRoleView.roleType;
            if (curRoleType == RoleType.MainRoleBoy)
            {
                img.sprite = bg_boy_normal;
            }
            else if (curRoleType == RoleType.MainRoleGirl)
            {
                img.sprite = bg_girl_normal;
            }
            text.text = config.name;
            rect.DOAnchorPosX(0, aniTime);
            itemID = config.ID;

       
        }
    }
}
