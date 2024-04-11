using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController
{
    public HomepageView homepageView;
    public LoadingView loadingView;
    public EpisodePlayerView episodePlayerView;
    public PhoneEpisodePlayerView phoneEpisodePlayerView;

    public void Init(GameObject canvas)
    {
        homepageView = canvas.GetComponentInChildren<HomepageView>();
        loadingView = canvas.GetComponentInChildren<LoadingView>();
        episodePlayerView = canvas.GetComponentInChildren<EpisodePlayerView>();
        phoneEpisodePlayerView = canvas.GetComponentInChildren<PhoneEpisodePlayerView>();

        loadingView.gameObject.SetActive(false);

        OpenHomepage();
    }

    public void OpenHomepage()
    {
        Debug.Log("´ò¿ªÊ×Ò³");
        ControllerManager.Get().UIController.homepageView?.gameObject.SetActive(true);

    }
}
