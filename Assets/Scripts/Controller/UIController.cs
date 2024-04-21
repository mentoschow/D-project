using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    // Ô¤ÖÆÌå
    public GameObject homepageObj;
    public GameObject loadingObj;
    public GameObject stageViewObj;
    public GameObject normalEpisodePlayerObj;
    public GameObject getItemTipObj;
    public GameObject phoneObj;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView normalEpisodePlayerView;
    private PhoneView phoneView;

    private Transform layer1;
    private Transform layer2;
    private Transform layer3;
    private Transform layer4;

    void Awake()
    {
        MessageManager.Instance.Register(MessageDefine.GameStart, GameStart);
        Init();
    }

    private void Update()
    {
        CheckInput();
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
        phoneView = CreateView<PhoneView>(phoneObj, layer2);

        HideAllView();
        OpenHomepage();
    }

    public void HideAllView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
        phoneView?.gameObject.SetActive(false);
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
        if (config.episodeType == EpisodeType.Normal)
        {
            normalEpisodePlayerView.gameObject.SetActive(true);
            normalEpisodePlayerView.PlayEpisode(ID);
        }
        else if (config.episodeType == EpisodeType.Phone)
        {
            phoneView.PlayPhoneEpisode(ID);
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

    public void GetItemTip(List<string> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            var obj = Instantiate(getItemTipObj);
            if (obj != null)
            {
                obj.transform.SetParent(layer4);
                var rect = obj.GetComponent<RectTransform>();
                float y = -((rect.rect.height + 50) * i + 100);
                rect.anchoredPosition = new Vector3(rect.rect.width, y);
                var view = obj.GetComponent<GetItemTipPartView>();
                view?.UpdateView(itemList[i]);
            }
        }
    }

    private void CheckInput()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            phoneView.ShowPhone();
        }
    }
}
