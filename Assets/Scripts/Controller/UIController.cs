using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    // Ԥ����
    public GameObject homepageObj;
    public GameObject loadingObj;
    public GameObject puzzleViewObj;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView episodePlayerView;
    private PuzzleView puzzleView;
    public GameObject stageViewObj;
    public GameObject normalEpisodePlayerObj;
    public GameObject getItemTipObj;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView normalEpisodePlayerView;

    private Transform layer1;
    private Transform layer2;
    private Transform layer3;
    private Transform layer4;

    void Awake()
    {
        MessageManager.Instance.Register(MessageDefine.GameStart, GameStart);
        Init();
    }

    void OnDestroy()
    {

    }

    private void Init()
    {
        // homepageView = CreateView<HomepageView>(homepageObj, transform.Find("layer1"));
        // loadingView = CreateView<LoadingView>(loadingObj, transform.Find("layer4"));
        // puzzleView = CreateView<PuzzleView>(puzzleViewObj, transform.Find("layer4"));
        layer1 = transform.Find("layer1");
        layer2 = transform.Find("layer2");
        layer3 = transform.Find("layer3");
        layer4 = transform.Find("layer4");
        homepageView = CreateView<HomepageView>(homepageObj, layer1);
        loadingView = CreateView<LoadingView>(loadingObj, layer4);
        normalEpisodePlayerView = CreateView<EpisodePlayerView>(normalEpisodePlayerObj, layer3);
        puzzleView = CreateView<PuzzleView>(puzzleViewObj, transform.Find("layer4"));
        
        HideAllView();
        OpenHomepage();
    }

    private void HideAllView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
    }

    private void OpenHomepage()
    {
        homepageView?.gameObject.SetActive(true);
    }

    public void GameStart(MessageData data)
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void GameEnd()
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
        normalEpisodePlayerView.gameObject.SetActive(false);
        if (config.episodeType == EpisodeType.Normal)
        {
            normalEpisodePlayerView.gameObject.SetActive(true);
            normalEpisodePlayerView.PlayEpisode(ID);
        }
        else if (config.episodeType == EpisodeType.Phone)
        {

        }
    }

    public T CreateView<T>(GameObject prefab, Transform parent)
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

    public void GetItemTip(List<string> itemList, Transform parent)
    {
        if (parent == null)
        {
            parent = layer4;
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            var obj = Instantiate(getItemTipObj);
            if (obj != null)
            {
                obj.transform.SetParent(parent);
                var rect = obj.GetComponent<RectTransform>();
                float y = -((rect.rect.height + 50) * i + 100);
                rect.anchoredPosition = new Vector3(0, y);
                var view = obj.GetComponent<GetItemTipPartView>();
                //view?.UpdateView(itemList[i]);
            }
        }
    }
}
