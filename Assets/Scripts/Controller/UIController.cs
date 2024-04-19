using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    // 预制体
    public GameObject homepageObj;
    public GameObject loadingObj;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView episodePlayerView;

    void Start()
    {
        Init();
    }

    void OnDestroy()
    {

    }

    private void Init()
    {
        homepageView = CreateView<HomepageView>(homepageObj, transform.Find("layer1"));
        loadingView = CreateView<LoadingView>(loadingObj, transform.Find("layer4"));
        //homepageView = Instantiate(homepageObj)?.GetComponent<HomepageView>();
        //homepageView?.transform.SetParent(transform.Find("layer1"));
        //loadingView = Instantiate(loadingObg)?.GetComponent<LoadingView>();
        //loadingView?.transform.SetParent(transform.Find("layer4"));

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
