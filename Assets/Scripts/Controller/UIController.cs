using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    // Ô¤ÖÆÌå
    public GameObject homepageObj;
    public GameObject loadingObj;
    public GameObject normalEpisodePlayerObj;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView normalEpisodePlayerView;

    private Transform layer1;
    private Transform layer2;
    private Transform layer3;
    private Transform layer4;

    void Start()
    {
        MessageManager.Instance.Register(MessageDefine.GameStart, GameStart);
        MessageManager.Instance.Register(MessageDefine.PlayTransitionDone, OpenStageView);
        Init();
    }

    void OnDestroy()
    {

    }

    private void Init()
    {
        layer1 = transform.Find("layer1");
        layer2 = transform.Find("layer2");
        layer3 = transform.Find("layer3");
        layer4 = transform.Find("layer4");
        homepageView = CreateView<HomepageView>(homepageObj, layer1);
        loadingView = CreateView<LoadingView>(loadingObj, layer4);
        normalEpisodePlayerView = CreateView<EpisodePlayerView>(normalEpisodePlayerObj, layer3);

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

    private void OpenStageView(MessageData data)
    {

    }

    public void GameStart(MessageData data)
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void GameEnd()
    {

    }

    public void ShowScene()
    {

    }

    public void ShowTransition(TransitionType type)
    {
        loadingView?.gameObject.SetActive(true);
        loadingView?.PlayTransition(type);
    }

    public void ShowTutorial()
    {

    }

    public void PlayEpisode(string ID)
    {
        var config = ConfigController.Instance.GetEpisodeConfig(ID);
        if (config.episodeType == EpisodeType.Normal)
        {
            normalEpisodePlayerView?.gameObject.SetActive(true);
            normalEpisodePlayerView?.PlayEpisode(ID);
        }
        else if (config.episodeType == EpisodeType.Phone)
        {

        }
    }

    private T CreateView<T>(GameObject prefab, Transform parent)
    {
        var obj = Instantiate(prefab);
        if (obj != null)
        {
            obj.transform.SetParent(parent);
            var rect = obj.GetComponent<RectTransform>();
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            var view = obj.GetComponent<T>();
            return view;
        }
        return default(T);
    }
}
