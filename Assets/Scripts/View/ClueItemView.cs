using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ClueItemView : MonoBehaviour
{
    [SerializeField]
    private RectTransform cluePage;
    [SerializeField]
    private GameObject detailPage;
    [SerializeField]
    private Text detailTitle;
    [SerializeField]
    private Text detailDescription;
    [SerializeField]
    private Image detailImage;
    [SerializeField]
    private Button closeDetailBtn;
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Text numText;
    [SerializeField]
    private Transform clueList;

    // Ô¤ÖÆÌå
    [SerializeField]
    private GameObject clueItemObj;

    private List<ClueItemPartView> itemViewList = new List<ClueItemPartView>();

    private const int showYPos = 528;

    void Start()
    {
        MessageManager.Instance.Register(MessageDefine.ClueItemClick, ShowDetail);
        closeBtn?.onClick.AddListener(Close);
        closeDetailBtn?.onClick.AddListener(CloseDetail);
        detailPage?.gameObject.SetActive(false);
    }

    public void UpdateView()
    {
        detailPage?.SetActive(false);
        var roleType = RoleController.Instance.curRoleView.roleType;
        List<string> items = new List<string>();
        if (roleType == RoleType.MainRoleGirl)
        {
            items = GameDataProxy.Instance.mainGirlBagItem;
        }
        else if (roleType == RoleType.MainRoleBoy)
        {
            items = GameDataProxy.Instance.mainBoyBagItem;
        }
        numText.text = items.Count.ToString() + "Æª±Ê¼Ç";
        foreach (var view in itemViewList)
        {
            view.gameObject.SetActive(false);
        }
        if (items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var view = BaseFunction.CreateView<ClueItemPartView>(clueItemObj);
                if (itemViewList.Count <= i)
                {
                    itemViewList.Add(view);
                    view.transform.SetParent(clueList);
                }
                else
                {
                    view = itemViewList[i];
                }
                if (view != null)
                {
                    view.gameObject.SetActive(true);
                    var config = ConfigController.Instance.GetClueItemConfig(items[i]);
                    view.UpdateView(config);
                }
            }
        }
    }

    private void ShowDetail(MessageData data)
    {
        string itemID = data.valueString;
        ItemConfig item = ConfigController.Instance.GetClueItemConfig(itemID);
        if (item != null )
        {
            detailPage?.SetActive(true);
            detailTitle.text = item.name;
            var text = BaseFunction.FixStringChangeLine(item.description);
            detailDescription.text = text;
            if (ResourcesController.Instance.clueItemRes.ContainsKey(item.ID))
            {
                detailImage.sprite = ResourcesController.Instance.clueItemRes[item.ID].sprite;
            }
        }
    }

    private void CloseDetail()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.PhoneButton);
        detailPage?.SetActive(false);
    }

    private void Close()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.PhoneButton);
        cluePage.anchoredPosition = new Vector2(cluePage.anchoredPosition.x, -showYPos);
        cluePage.DOAnchorPosY(-showYPos, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        cluePage.anchoredPosition = new Vector2(cluePage.anchoredPosition.x, -showYPos);
        cluePage.DOAnchorPosY(showYPos, 0.4f);
    }
}
