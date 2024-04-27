using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ClueItemView : MonoBehaviour
{
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
        var items = GameDataProxy.Instance.bagItem;
        numText.text = items.Count.ToString() + "Æª±Ê¼Ç";
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
                    view.UpdateView(items[i]);
                }
            }
        }
    }

    private void ShowDetail(MessageData data)
    {
        string itemID = data.valueString;
        ItemConfig item = ConfigController.Instance.GetItemConfig(itemID);
        if (item != null )
        {
            detailPage?.SetActive(true);
            detailTitle.text = item.name;
            detailDescription.text = item.description;
            if (ResourcesController.Instance.clueItemRes.ContainsKey(item.ID))
            {
                detailImage.sprite = ResourcesController.Instance.clueItemRes[item.ID].sprite;
            }
        }
    }

    private void CloseDetail()
    {
        detailPage?.SetActive(false);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
