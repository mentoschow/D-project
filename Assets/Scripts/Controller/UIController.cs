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
        Init();
    }

    void OnDestroy()
    {

    }

    public void Init()
    {
        homepageView = GetComponentInChildren<HomepageView>();
        loadingView = GetComponentInChildren<LoadingView>();
        episodePlayerView = GetComponentInChildren<EpisodePlayerView>();
        phoneEpisodePlayerView = GetComponentInChildren<PhoneEpisodePlayerView>();

        HideAllView();

        OpenHomepage();
    }

    private void HideAllView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
    }

    public void OpenHomepage()
    {
        homepageView?.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void OpenTransitionView(TransitionType type)
    {
        Debug.Log("进入转场" + type);
    }
}
