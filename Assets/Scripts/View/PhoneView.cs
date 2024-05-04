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
    private Button closeBtn;

    // 信号切换的图
    [SerializeField]
    private GameObject signal;
    [SerializeField] 
    private GameObject nosignal;

    // 边边按钮
    [SerializeField]
    private Button backBtn;
    [SerializeField]
    private Button fastItemBtn;
    [SerializeField] 
    private Button fastWechatBtn;

    // 主页
    [SerializeField]
    private GameObject phoneHomepage;
    [SerializeField]
    private Button itemBtn;
    [SerializeField]
    private Button wechatBtn;
    [SerializeField]
    private GameObject wechatBtnRedPoint;

    // 线索
    [SerializeField] 
    private ClueItemView cluePage;

    // 聊聊
    [SerializeField]
    private PhoneWechatView wechatPage;

    // 聊聊对话
    [SerializeField]
    private GameObject dialogRoot;
    [SerializeField]
    private Transform wechatDialogPage;
    [SerializeField]
    private Text dialogGroupName;

    // 资源
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
        dialogRoot?.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(true);
        fastItemBtn.gameObject.SetActive(false);
        fastWechatBtn.gameObject.SetActive(false);
        curPage = PhonePageType.Homepage;
    }

    public void PlayPhoneEpisode(EpisodeConfig config)
    {
        gameObject.SetActive(true);
        ShowEpisodeView(config.belongGroup, config.ID);
    }

    private void ShowWechat()
    {
        AudioController.Instance.PlayAudioEffect(AudioEffectType.PhoneButton);
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        cluePage?.gameObject.SetActive(false);
        dialogRoot?.gameObject?.SetActive(false);
        wechatPage?.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        fastItemBtn.gameObject.SetActive(true);
        fastWechatBtn.gameObject.SetActive(true);
        curPage = PhonePageType.Wechat;
        wechatPage.UpdateView();
    }

    private void ShowEpisodeView(BelongPhoneGroup group, string ID)
    {
        dialogRoot.gameObject.SetActive(true);
        cluePage?.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(false);
        fastItemBtn.gameObject.SetActive(false);
        fastWechatBtn.gameObject.SetActive(false);
        curPage = PhonePageType.WechatDialog;
        if (!rolePhoneDialogGroupList.ContainsKey(group))
        {
            var playerView = BaseFunction.CreateView<EpisodePlayerView>(wechatDialogPageObj, wechatDialogPage);
            rolePhoneDialogGroupList[group] = playerView;
        }
        var player = rolePhoneDialogGroupList[group];
        player.gameObject.SetActive(true);
        player.PlayEpisode(ID);
        if (ResourcesController.Instance.wechatGroupRes.ContainsKey(group))
        {
            string name = ResourcesController.Instance.wechatGroupRes[group].name;
            dialogGroupName.text = name;
        }
    }

    private void OnOpenWechatDialogPage(MessageData data)
    {
        dialogRoot.gameObject.SetActive(true);
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
        if (ResourcesController.Instance.wechatGroupRes.ContainsKey(group))
        {
            string name = ResourcesController.Instance.wechatGroupRes[group].name;
            dialogGroupName.text = name;
        }
    }

    private void ShowItemView()
    {
        AudioController.Instance.PlayAudioEffect(AudioEffectType.PhoneButton);
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        curPage = PhonePageType.Clue;
        wechatPage?.gameObject.SetActive(false);
        dialogRoot?.gameObject.SetActive(false);
        cluePage?.gameObject.SetActive(true);
        backBtn.gameObject.SetActive(true);
        fastItemBtn.gameObject.SetActive(true);
        fastWechatBtn.gameObject.SetActive(true);

        cluePage.UpdateView();
    }

    private void BackPage()
    {
        AudioController.Instance.PlayAudioEffect(AudioEffectType.PhoneButton);
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
                dialogRoot.gameObject.SetActive(false);
                curPage = PhonePageType.Wechat;
                break;
        }
    }

    private void Close()
    {
        AudioController.Instance.PlayAudioEffect(AudioEffectType.PhoneButton);
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        gameObject.SetActive(false);
    }
}
