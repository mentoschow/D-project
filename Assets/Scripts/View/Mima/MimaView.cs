using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MimaView : MonoSingleton<MimaView>
{
    GameObject open_pic;
    GameObject close_pic;
    private Button closeBtn;
    private Button finishBtn;

    int count = 7;
    int originNumber = 0;
    Dictionary<int,MimaItemContainerView> itemViewMap = new Dictionary<int,MimaItemContainerView>();
    Dictionary<int, List<string>> urlMap = new Dictionary<int, List<string>> {
        {0,new List<string> { "杨","拨","东","春","伊" } },
        {1,new List<string> { "花","娇","人","尽","来" } },
        {2,new List<string>  { "浮","百","难","无","志" } },
        {3,new List<string>  { "媚","情","阻","云","岂" } },
        {4,new List<string>  { "却","肝","青","在","览" } },
        {5,new List<string> { "阑","肠","封","云","众" } },
        {6,new List<string>  { "断","志","山","侯","珊" } },
    };


    static GameObject ViewPrefab;
    public static GameObject getPrefab()
    {
        if (!MimaView.ViewPrefab)
        {
            string resourceName = "Prefabs/UI/Mima/MimaView"; // 资源名称
            MimaView.ViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return MimaView.ViewPrefab;
    }

    public void onCloseBtnClick()
    {
        gameObject.SetActive(false);
        GameDataProxy.Instance.canOperate = true;
    }

    void Start()
    {
        closeBtn?.onClick.AddListener(onCloseBtnClick); 
    }

    private void Awake()
    {
        closeBtn = CommonUtils.findChildByName(transform, "closeBtn").gameObject.GetComponent<UnityEngine.UI.Button>();
        closeBtn?.onClick.AddListener(onCloseBtnClick);
        finishBtn = transform.Find("finishBtn")?.GetComponent<Button>();
        finishBtn?.onClick.AddListener(OnFinishClick);
#if UNITY_EDITOR
        finishBtn.gameObject.SetActive(true);
#else
        finishBtn.gameObject.SetActive(false);
#endif
        List<int> posList = new List<int> {2,3,4,0,1 };
        for (int i = 0;i< count; i++)
        {
            string nodeName = "rollNode_"+ i.ToString();
            GameObject attachNode = CommonUtils.findChildByName(transform,nodeName).gameObject;
            MimaItemContainerView itemView = attachNode.AddComponent<MimaItemContainerView>();
            itemViewMap.Add(i, itemView);

            List<string> showList = new List<string>();
            this.urlMap.TryGetValue(i, out showList);
            itemView.init(originNumber, () =>
            {
                this.checkRight();
            },showList,posList);
        }
        open_pic = CommonUtils.findChildByName(transform, "open_pic").gameObject;
        close_pic = CommonUtils.findChildByName(transform, "close_pic").gameObject;
    }

    public void updateView()
    {
        bool isOver = GameDataProxy.Instance.checkMimaOver();
        open_pic.SetActive(isOver);
        close_pic.SetActive(!isOver);
        if (isOver)
        {
            foreach (KeyValuePair<int, MimaItemContainerView> item in itemViewMap)
            {
                item.Value.isCanUse  = false;
            }
        }
    }

    public void checkRight(bool over = false) {
        List<int> list = new List<int>();

        foreach (KeyValuePair<int, MimaItemContainerView> item in itemViewMap)
        {
            list.Add(item.Value.curNumber + 1);
        }
        GameDataProxy.Instance.useMimaList = list;
        bool isOver  = GameDataProxy.Instance.checkMimaOver();
        if (over)
        {
            isOver = true;
        }
        if (isOver)
        {
            //AudioController.Instance.PlayAudioEffect(AudioType.PuzzleCorrect);
            AudioController.Instance.PlayAudioEffect(AudioType.Unlock);
            this.updateView();
            Debug.Log("密码锁已完成");
            //UIController.Instance.GetItemTip(new List<string>() { "CUE_0350" });
            GameLineNode lineNode = new GameLineNode();
            lineNode.type = GameNodeType.Puzzle;
            lineNode.ID = PuzzleType.PasswordPuzzle.ToString();
            MessageManager.Instance.Send(MessageDefine.GameLineNodeDone, new MessageData(lineNode));

            gameObject.SetActive(false);
        }
    }

    private void OnFinishClick()
    {
        checkRight(true);
    }
}
