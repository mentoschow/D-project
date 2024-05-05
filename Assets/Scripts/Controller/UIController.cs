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


    [SerializeField]
    private GameObject jiguanguiObj;
    GameObject testControlViewPrefab;

    public GameObject normalEpisodePlayerObj;
    public GameObject getItemTipObj;
    public GameObject phoneObj;
    public GameObject clueItemViewObj;
    public GameObject commonButtonPartObj;

    private GameJiguanguiView jiguanguiView;
    private MimaView mimaView;
    private ClueCombineView clueCombineView;
    private ClueCombineView clueCombinePhoneView;

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
        commonButtonPartView = BaseFunction.CreateView<CommonButtonPartView>(commonButtonPartObj, layer1);
        homepageView = CreatePanelView<HomepageView>(homepageObj, layer1);
        loadingView = BaseFunction.CreateView<LoadingView>(loadingObj, layer4);
        normalEpisodePlayerView = CreatePanelView<EpisodePlayerView>(normalEpisodePlayerObj, layer3);
        phoneView = CreatePanelView<PhoneView>(phoneObj, layer2);
        mainRoleBoyClueView = CreatePanelView<ClueItemView>(clueItemViewObj, layer2);

        ShowHomepage();

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

    public void HideGamePlayView()
    {
        homepageView?.gameObject.SetActive(false);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
        phoneView?.gameObject.SetActive(false);
        jiguanguiView?.gameObject.SetActive(false);
        mainRoleBoyClueView?.gameObject.SetActive(false);
        commonButtonPartView?.gameObject.SetActive(true);
    }

    public void ShowHomepage(bool isGameOver = false)
    {
        homepageView?.gameObject.SetActive(true);
        loadingView?.gameObject.SetActive(false);
        normalEpisodePlayerView?.gameObject.SetActive(false);
        phoneView?.gameObject.SetActive(false);
        jiguanguiView?.gameObject.SetActive(false);
        mainRoleBoyClueView?.gameObject.SetActive(false);
        commonButtonPartView?.gameObject.SetActive(false);
        if (isGameOver)
        {
            homepageView.ShowGameOver();
        }
    }

    public void showJiguanguiView()
    {
        if (jiguanguiView == null)
        {
            jiguanguiView = CreatePanelView<GameJiguanguiView>(jiguanguiObj, layer2);
        }

        jiguanguiView?.gameObject.SetActive(true);
    }


    public void showMimaView()
    {
        if (mimaView == null)
        {
            mimaView = CommonUtils.CreateViewByType<MimaView>(MimaView.getPrefab(), layer2);
        }

        mimaView.gameObject.SetActive(true);
        mimaView.updateView();
    }

    public void showClueCombineView(MergeClueConfig mergeClueConfig)
    {
        var curRoleType = RoleController.Instance.curRoleView.roleType;
        bool isBoy = curRoleType == RoleType.MainRoleBoy;
        if (isBoy)
        {
            if (clueCombineView == null)
            {
                clueCombineView = CommonUtils.CreateViewByType<ClueCombineView>(ClueCombineView.getPrefab(curRoleType), layer3);
            }

            clueCombineView.gameObject.SetActive(true);
            clueCombineView.updateView(mergeClueConfig);
        }
        else
        {
            if (clueCombinePhoneView == null)
            {
                clueCombinePhoneView = CommonUtils.CreateViewByType<ClueCombineView>(ClueCombineView.getPrefab(curRoleType), layer3);
            }

            clueCombinePhoneView.gameObject.SetActive(true);
            clueCombinePhoneView.updateView(mergeClueConfig);
        }
    }

    public void GameStart(MessageData data)
    {
        homepageView?.gameObject.SetActive(false);
    }

    public void GameEnd()
    {
        BackHomepage(true);
    }

    public void BackHomepage(bool isGameOver = false)
    {
        ShowHomepage(isGameOver);
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
        phoneView?.CloseView();
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
                    if (!GameDataProxy.Instance.mainGirlBagItem.Contains(id))
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

    private void OnGetItemDone()
    {
        getItemTipIndex--;
        if (getItemTipIndex < 0)
        {
            getItemTipIndex = 0;
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //ShowPhoneOrClue();
        }
    }

    public void ShowPhoneOrClue()
    {
        if (!GameDataProxy.Instance.canOperate)
        {
            return;
        }
        var roleType = RoleController.Instance.curRoleView.roleType;
        if (roleType == RoleType.MainRoleGirl)
        {
            phoneView.ShowPhone();
        }
        else if (roleType == RoleType.MainRoleBoy)
        {
            mainRoleBoyClueView.Show();
            mainRoleBoyClueView.UpdateView();
        }
    }
}
