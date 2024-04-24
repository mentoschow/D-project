using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class PhoneView : MonoBehaviour
{
    public enum PhonePageType
    {
        Homepage,
        Wechat,
        WechatDialog,
        Clue,
    }

    [SerializeField]
    private Button itemBtn;
    [SerializeField]
    private Button wechatBtn;
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Button backBtn;
    [SerializeField]
    private Button fastItemBtn;
    [SerializeField] 
    private Button fastWechatBtn;
    [SerializeField]
    private GameObject phoneHomepage;
    [SerializeField] 
    private Transform cluePage;
    [SerializeField]
    private PhoneWechatView wechatPage;
    [SerializeField]
    private Transform wechatDialogPage;

    // ×ÊÔ´
    [SerializeField]
    private GameObject wechatDialogPageObj;

    private Dictionary<BelongPhoneGroup, EpisodePlayerView> rolePhoneDialogGroupList = new Dictionary<BelongPhoneGroup, EpisodePlayerView>();
    private PhonePageType curPage;

    void Awake()
    {
        itemBtn?.onClick.AddListener(ShowItemView);
        wechatBtn?.onClick.AddListener(ShowWechat);
        closeBtn?.onClick.AddListener(Close);
        fastItemBtn?.onClick.AddListener(ShowItemView);
        fastWechatBtn?.onClick.AddListener(ShowWechat);
        backBtn?.onClick.AddListener(BackPage);
        MessageManager.Instance.Register(MessageDefine.OpenWechatDialogPage, OnOpenWechatDialogPage);
    }

    public void ShowPhone()
    {
        gameObject.SetActive(true);
        phoneHomepage?.SetActive(true);
        cluePage?.gameObject.SetActive(false);
        wechatPage?.gameObject.SetActive(false);
        wechatDialogPage?.gameObject.SetActive(false);
        curPage = PhonePageType.Homepage;
    }

    public void PlayPhoneEpisode(EpisodeConfig config)
    {
        gameObject.SetActive(true);
        ShowEpisodeView(config.belongGroup, config.ID);
    }

    private void ShowWechat()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        cluePage?.gameObject.SetActive(false);
        wechatDialogPage?.gameObject?.SetActive(false);
        wechatPage?.gameObject.SetActive(true);
        wechatPage.UpdateView();
        curPage = PhonePageType.Wechat;
    }

    private void ShowEpisodeView(BelongPhoneGroup group, string ID)
    {
        wechatDialogPage.gameObject.SetActive(true);
        if (!rolePhoneDialogGroupList.ContainsKey(group))
        {
            var playerView = BaseFunction.CreateView<EpisodePlayerView>(wechatDialogPageObj, wechatDialogPage);
            rolePhoneDialogGroupList[group] = playerView;
        }
        var player = rolePhoneDialogGroupList[group];
        player.gameObject.SetActive(true);
        player.PlayEpisode(ID);
        curPage = PhonePageType.WechatDialog;
    }

    private void OnOpenWechatDialogPage(MessageData data)
    {
        wechatDialogPage.gameObject.SetActive(true);
        var group = (BelongPhoneGroup)data.valueObject;
        foreach (var view in rolePhoneDialogGroupList) 
        {
            if (view.Key == group)
            {
                view.Value.gameObject.SetActive(true);
                curPage = PhonePageType.WechatDialog;
            }
            else
            {
                view.Value.gameObject.SetActive(false);
            }
        }
    }

    private void ShowItemView()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        wechatPage?.gameObject.SetActive(false);
        wechatDialogPage?.gameObject.SetActive(false);
        cluePage?.gameObject.SetActive(true);
        curPage = PhonePageType.Clue;
    }

    private void BackPage()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        switch (curPage)
        {
            case PhonePageType.Homepage:
                Close();
                break;
            case PhonePageType.Clue:
                cluePage.gameObject.SetActive(false);
                curPage = PhonePageType.Homepage;
                break;
            case PhonePageType.Wechat:
                wechatPage?.gameObject.SetActive(false);
                curPage = PhonePageType.Homepage;
                break;
            case PhonePageType.WechatDialog:
                wechatDialogPage.gameObject.SetActive(false);
                curPage = PhonePageType.Wechat;
                break;
        }
    }

    private void Close()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        gameObject.SetActive(false);
    }
}
