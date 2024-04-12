using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    public HomepageView homepageView;
    public LoadingView loadingView;
    public EpisodePlayerView episodePlayerView;
    public PhoneEpisodePlayerView phoneEpisodePlayerView;

    void Start()
    {
        MessageManager.Instance.Register(MessageDefine.GameStart, GameStart);
        Init();
    }

    void OnDestroy()
    {
        MessageManager.Instance.Remove(MessageDefine.GameStart, GameStart);
    }

    public void Init()
    {
        homepageView = GetComponentInChildren<HomepageView>();
        loadingView = GetComponentInChildren<LoadingView>();
        episodePlayerView = GetComponentInChildren<EpisodePlayerView>();
        phoneEpisodePlayerView = GetComponentInChildren<PhoneEpisodePlayerView>();

        loadingView.gameObject.SetActive(false);

        OpenHomepage();
    }

    public void OpenHomepage()
    {
        Debug.Log("����ҳ");
        homepageView.gameObject.SetActive(true);
    }

    void GameStart()
    {
        Debug.Log("�յ���Ϣ����Ϸ��ʼ��");
        homepageView.gameObject.SetActive(false);
    }
}
