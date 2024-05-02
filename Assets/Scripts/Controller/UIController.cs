using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoSingleton<UIController>
{
    public GameObject homepageObj;
    public GameObject loadingObj;

    GameObject puzzleViewPrefab;
    GameObject testControlViewPrefab;

    public GameObject normalEpisodePlayerObj;
    public GameObject getItemTipObj;
    public GameObject phoneObj;
    public GameObject clueItemViewObj;
    public GameObject commonButtonPartObj;

    private PuzzleView puzzleView;
    private MimaView mimaView;

    private TestControlView testControlView;

    private HomepageView homepageView;
    private LoadingView loadingView;
    private EpisodePlayerView normalEpisodePlayerView;
    private PhoneView phoneView;
    private ClueItemView mainRoleBoyClueView;
    private CommonButtonPartView commonButtonPartView;

    private Transform layer1;
    private Transform layer2;
    private Transform layer3;
    private Transform layer4;
    private int getItemTipIndex = 0;

    public Button testBtn;
    void Awake()
    {
        MessageManager.Instance.Register(MessageDefine.StageStart, GameStart);
        MessageManager.Instance.Register(MessageDefine.GetItemDone, OnGetItemDone);
        Init();
    }

    private void Update()
    {
        CheckInput();
    }

    private void Init()
    {

        DOTween.Init();

        layer1 = transform.Find("layer1");
        layer2 = transform.Find("layer2");
        layer3 = transform.Find("layer3");
        layer4 = transform.Find("layer4");
        commonButtonPartView = BaseFunction.CreateView<CommonButtonPartView>(commonButtonPartObj, layer2);
        homepageView = CreatePanelView<HomepageView>(homepageObj, layer1);
        loadingView = BaseFunction.CreateView<LoadingView>(loadingObj, layer4);
        normalEpisodePlayerView = CreatePanelView<EpisodePlayerView>(normalEpisodePlayerObj, layer3);
        phoneView = CreatePanelView<PhoneView>(phoneObj, layer2);
        mainRoleBoyClueView = CreatePanelView<ClueItemView>(clueItemViewObj, layer2);

        HideGamePlayView();

        testBtn.onClick.AddListener(onTestBtnClick);
    }

     void onTestBtnClick()
    {
        if (testControlViewPrefab == null)
        {
            string resourceName = "Prefabs/UI/TestControlView"; // 资源名称
            testControlViewPrefab = Resources.Load<GameObject>(resourceName);
        }
        if (testControlView == null)
        {
            testControlView = CreatePanelView<TestControlView>(testControlViewPrefab, layer4);
        }

        testControlView?.gameObject.SetActive(true);
    }

    public void ShowGamePlayView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
        phoneView?.gameObject.SetActive(false);
        puzzleView?.gameObject.SetActive(false);
        mainRoleBoyClueView?.gameObject.SetActive(false);
        commonButtonPartView?.gameObject.SetActive(true);
    }

    public void HideGamePlayView()
    {
        homepageView?.gameObject.SetActive(true);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
        phoneView?.gameObject.SetActive(false);
        puzzleView?.gameObject.SetActive(false);
        mainRoleBoyClueView?.gameObject.SetActive(false);
        commonButtonPartView?.gameObject.SetActive(false);
    }

    public void showPuzzleView()
    {
        if (puzzleViewPrefab == null)
        {
            string resourceName = "Prefabs/UI/puzzle/GamePuzzleView"; // 资源名称
            puzzleViewPrefab = Resources.Load<GameObject>(resourceName);
        }
        if (puzzleView == null)
        {
            puzzleView = CreatePanelView<PuzzleView>(puzzleViewPrefab, layer4);
        }

        puzzleView?.gameObject.SetActive(true);
        var config = ConfigController.Instance.puzzleConfig;
        if (config != null)
        {
            puzzleView?.updateView(config);
        }
    }


    public void showMimaView()
    {
        if (mimaView == null)
        {
            mimaView = CommonUtils.CreateViewByType<MimaView>(MimaView.getPrefab(), layer4);
        }

        mimaView.gameObject.SetActive(true);
        mimaView.updateView();
    }

    public void GameStart(MessageData data)
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void GameEnd()
    {
        BackHomepage();
    }

    public void BackHomepage()
    {
        HideGamePlayView();
        GameDataProxy.Instance.ResetData();
        SceneController.Instance.ResetData();
    }

    public void ShowTransition(string str)
    {
        loadingView?.gameObject.SetActive(true);
        loadingView?.PlayTransition(str);
    }

    public void ShowTransition(TransitionType type)
    {
        loadingView?.gameObject.SetActive(true);
        loadingView?.PlayTransition(type);
    }

    public void HidePhoneView()
    {
        phoneView?.gameObject.SetActive(false);
    }

    public void PlayEpisode(string ID)
    {
        var config = ConfigController.Instance.GetEpisodeConfig(ID);
        if (config != null)
        {
            bool canPlay = true;
            if (config.needFinishEpisodeID?.Count > 0)
            {
                foreach (var id in config.needFinishEpisodeID)
                {
                    if (!GameDataProxy.Instance.finishedEpisode.Contains(id))
                    {
                        canPlay = false;
                        break;
                    }
                }
            }
            if (config.needItemID?.Count > 0)
            {
                foreach (var id in config.needItemID)
                {
                    if (!GameDataProxy.Instance.mainGrilBagItem.Contains(id))
                    {
                        canPlay = false;
                        break;
                    }
                }
            }
            if (canPlay)
            {
                GameDataProxy.Instance.canOperate = false;
                if (config.episodeType == EpisodeType.Normal)
                {
                    normalEpisodePlayerView.gameObject.SetActive(true);
                    normalEpisodePlayerView.PlayEpisode(ID);
                }
                else if (config.episodeType == EpisodeType.Phone)
                {
                    phoneView.PlayPhoneEpisode(config);
                }
            }
            else
            {
                GameDataProxy.Instance.canOperate = true;
            }
        }
    }

    private T CreatePanelView<T>(GameObject prefab, Transform parent)
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
            if (GameDataProxy.Instance.CheckHasClueItem(itemList[i], RoleController.Instance.curRoleView.roleType))
            {
                // 避免重复获取
                continue;
            }
            var config = ConfigController.Instance.GetClueItemConfig(itemList[i]);
            if (config != null)
            {
                if (!config.isSaveBag)
                {
                    continue;
                }
            }
            var obj = Instantiate(getItemTipObj);
            if (obj != null)
            {
                obj.transform.SetParent(layer4);
                var rect = obj.GetComponent<RectTransform>();
                float y = -((rect.rect.height + 20) * getItemTipIndex + 100);
                rect.anchoredPosition = new Vector3(rect.rect.width, y);
                var view = obj.GetComponent<GetItemTipPartView>();
                view?.UpdateView(itemList[i]);
                getItemTipIndex++;
            }
        }
    }

    private void OnGetItemDone(MessageData data)
    {
        getItemTipIndex--;
        if (getItemTipIndex < 0)
        {
            getItemTipIndex = 0;
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
