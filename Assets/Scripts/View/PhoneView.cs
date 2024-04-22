using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class PhoneView : MonoBehaviour
{
    [SerializeField]
    private Button itemBtn;
    [SerializeField]
    private Button episodeBtn;
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private GameObject phoneHomepage;
    [SerializeField]
    private PhoneWechatView wechatPage;
    [SerializeField]
    private Transform wechatDialogPage;
    [SerializeField]
    private GameObject wechatDialogPageObj;

    private Dictionary<BelongPhoneGroup, EpisodePlayerView> rolePhoneDialogGroupList = new Dictionary<BelongPhoneGroup, EpisodePlayerView>();

    void Awake()
    {
        itemBtn?.onClick.AddListener(ShowItemView);
        episodeBtn?.onClick.AddListener(ShowWechat);
        closeBtn?.onClick.AddListener(Close);
        MessageManager.Instance.Register(MessageDefine.OpenWechatDialogPage, OnOpenWechatDialogPage);
    }

    public void ShowPhone()
    {
        gameObject.SetActive(true);
        phoneHomepage?.SetActive(true);
        wechatPage?.gameObject.SetActive(false);
        wechatDialogPage?.gameObject.SetActive(false);
        closeBtn.interactable = true;
    }

    public void PlayPhoneEpisode(EpisodeConfig config)
    {
        gameObject.SetActive(true);
        closeBtn.interactable = false;
        ShowEpisodeView(config.belongGroup, config.ID);
    }

    private void ShowWechat()
    {
        wechatPage?.gameObject.SetActive(true);
        wechatPage.UpdateView();
    }

    private void ShowEpisodeView(BelongPhoneGroup group, string ID)
    {
        if (!rolePhoneDialogGroupList.ContainsKey(group))
        {
            var playerView = BaseFunction.CreateView<EpisodePlayerView>(wechatDialogPageObj, wechatDialogPage);
            rolePhoneDialogGroupList[group] = playerView;
        }
        var player = rolePhoneDialogGroupList[group];
        player.gameObject.SetActive(true);
        player.PlayEpisode(ID);
    }

    private void OnOpenWechatDialogPage(MessageData data)
    {
        var group = (BelongPhoneGroup)data.valueObject;
        foreach (var view in rolePhoneDialogGroupList) 
        {
            if (view.Key == group)
            {
                view.Value.gameObject.SetActive(true);
            }
            else
            {
                view.Value.gameObject.SetActive(false);
            }
        }
    }

    private void ShowItemView()
    {

    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
