using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    public HomepageView homepageView;
    public LoadingView loadingView;
    public EpisodePlayerView episodePlayerView;

    void Start()
    {
        Init();
    }

    void OnDestroy()
    {

    }

    private void Init()
    {
        homepageView = GetComponentInChildren<HomepageView>();
        loadingView = GetComponentInChildren<LoadingView>();
        episodePlayerView = GetComponentInChildren<EpisodePlayerView>();
        HideAllView();
        OpenHomepage();
    }


    private void HideAllView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
    }

    private void OpenHomepage()
    {
        homepageView?.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void ShowScene()
    {

    }

    public void ShowTransition(string ID)
    {

    }

    public void OpenTransitionView(TransitionType type)
    {
        Debug.Log("进入转场" + type);
    }
}
