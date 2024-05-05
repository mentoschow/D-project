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
    int originNumber = 2;
    Dictionary<int,MimaItemContainerView> itemViewMap = new Dictionary<int,MimaItemContainerView>();

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

        for (int i = 0;i< count; i++)
        {
            string nodeName = "rollNode_"+ i.ToString();
            GameObject attachNode = CommonUtils.findChildByName(transform,nodeName).gameObject;
            MimaItemContainerView itemView = attachNode.AddComponent<MimaItemContainerView>();
            itemViewMap.Add(i, itemView);

            itemView.init(originNumber, () =>
            {
                this.checkRight();
            });
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
