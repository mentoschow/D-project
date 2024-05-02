using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GetItemTipPartView : MonoBehaviour
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


    void Awake()
    {
        rect = GetComponent<RectTransform>();
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
            CheckSaveBag(config);
        }
        Invoke("Disappear", stayTime);
    }

    private void Disappear()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rect.DOAnchorPosX(rect.rect.width, aniTime)).AppendCallback(() =>
        {
            MessageManager.Instance.Send(MessageDefine.GetItemDone);
            Destroy(gameObject);
        });
    }

    private void CheckSaveBag(ItemConfig config)
    {
        if (config.isSaveBag)
        {
            var roleType = RoleController.Instance.curRoleView.roleType;
            if (roleType == RoleType.MainRoleGirl)
            {
                GameDataProxy.Instance.mainGirlBagItem.Add(config.ID);
            }
            else if (roleType == RoleType.MainRoleBoy)
            {
                GameDataProxy.Instance.mainBoyBagItem.Add(config.ID);
            }
        }
    }
}
