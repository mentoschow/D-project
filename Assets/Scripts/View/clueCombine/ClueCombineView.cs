using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ClueCombineView : MonoSingleton<ClueCombineView>
{
    // Start is called before the first frame update
    public GameObject layout_right;
    public GameObject layout_left;
    public GameObject layout_contain;
    //public Button closeBtn;
    public GameObject curDragAttachNode;
    public GameObject right_node;
    public Text rightTxt;
    public Text questionTxt;

    Dictionary<int, ClueCombineContainView> itemViewMap = new Dictionary<int, ClueCombineContainView>();

    Dictionary<int, string> combineMap = new Dictionary<int, string>();
    List<string> rightcombineList = new List<string>();

    int rightCout = 0;

    Dictionary<string, ClueCombineItemView> clueItemViewMap = new Dictionary<string, ClueCombineItemView>();

    string  moveItemCode = "";
    GameObject curDragNode = null;

    static Dictionary<RoleType, GameObject> ViewPrefabMap = new Dictionary<RoleType, GameObject>();
    public static GameObject getPrefab(RoleType curRoleType)
    {
        bool isBoy = curRoleType == RoleType.MainRoleBoy;
        GameObject viewPrefab;
        ViewPrefabMap.TryGetValue(curRoleType, out viewPrefab);
        if (!viewPrefab)
        {
            string resourceName = isBoy?"Prefabs/UI/ClueCombine/ClueCombineView": "Prefabs/UI/ClueCombine/ClueCombinePhoneView"; // 资源名称
            viewPrefab = Resources.Load<GameObject>(resourceName);
            ViewPrefabMap.Add(curRoleType, viewPrefab);
        }

        return viewPrefab;
    }

    private void Awake()
    {
        layout_right = CommonUtils.findChildByName(transform, "layout_right").gameObject;
        layout_left = CommonUtils.findChildByName(transform, "layout_left").gameObject;
        layout_contain = CommonUtils.findChildByName(transform, "layout_contain").gameObject;
        curDragAttachNode = CommonUtils.findChildByName(transform, "curDragAttachNode").gameObject;
        rightTxt = CommonUtils.findChildByName(transform, "rightTxt").gameObject.GetComponent<Text>();
        right_node = CommonUtils.findChildByName(transform, "right_node").gameObject;
        questionTxt = CommonUtils.findChildByName(transform, "questionTxt").gameObject.GetComponent<Text>();

        //closeBtn = CommonUtils.findChildByName(transform, "closeBtn").gameObject.GetComponent<UnityEngine.UI.Button>();
        //closeBtn?.onClick.AddListener(onCloseBtnClick);
    }

    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }

    void initAttachNodeMap()
    {
        {
            for (int i = 0; i < this.rightCout; i++)
            {
                ClueCombineContainView itemView = CommonUtils.CreateViewByType<ClueCombineContainView>(ClueCombineContainView.getPrefab(), layout_contain.transform);
                itemViewMap.Add(i, itemView);
            }
        }
    }

    void resetView()
    {
        rightcombineList = new List<string>();
        combineMap = new Dictionary<int, string>();

        this.right_node.SetActive(false);
        this.layout_contain.gameObject.SetActive(true);

        foreach (KeyValuePair<string, ClueCombineItemView> item in clueItemViewMap)
        {
            Destroy(item.Value.gameObject);
        }
        clueItemViewMap = new Dictionary<string, ClueCombineItemView>();
        foreach (KeyValuePair<int, ClueCombineContainView> item in itemViewMap)
        {
            Destroy(item.Value.gameObject);
        }
        itemViewMap = new Dictionary<int, ClueCombineContainView>();
    }

    MergeClueConfig mergeClueConfig;
    public void updateView(MergeClueConfig merClueConfig)
    {
        this.resetView();

        this.mergeClueConfig = merClueConfig;
        this.rightCout = this.mergeClueConfig.correctClueList.Count;

        CommonUtils.updateText(this.mergeClueConfig?.ID ?? "", this.questionTxt);

        this.initAttachNodeMap();

        rightcombineList = new List<string>(merClueConfig.correctClueList);

        this.updateClue(this.mergeClueConfig.prepareClueList);
    }

    public void updateClue(List<string> clueList)
    {

        for (int i = 0; i < clueList.Count; i++)
        {
            string clueID = clueList[i];
            GameObject layout = (i % 2 == 0) ? layout_left : layout_right;
            ClueCombineItemView itemView = null;
            bool isExist = clueItemViewMap.TryGetValue(clueID, out itemView);
            if (!isExist)
            {
                itemView = CommonUtils.CreateViewByType<ClueCombineItemView>(ClueCombineItemView.getPrefab(), layout?.transform);
                clueItemViewMap.Add(clueID, itemView);
            }
            Action<GameObject, int> dragStartCallback = (GameObject dragNode, int code) => {
                this.moveItemCode = clueID;
                if (this.curDragNode)
                {
                    Destroy(this.curDragNode);
                }
                this.curDragNode = Instantiate(dragNode, curDragAttachNode.transform);
            };
            Action<Vector2> dragMoveCallback = (Vector2 pos) =>
            {
                this.curDragNode.transform.position = pos;
            };
            Action<Vector2, int> dragOverCallback = (Vector2 inputPos, int code) =>
            {
                this.moveItemCode = "";
                if (this.curDragNode)
                {
                    Destroy(this.curDragNode);
                }

                this.checkCanInsert(clueID, inputPos);
                this.refreshClueItemState();
                this.refreshContainItemState();
                this.checkInsertOver();

            };

            itemView.UpdateView(clueID);
            itemView.updateDrag(dragStartCallback, dragMoveCallback, dragOverCallback);
        }
        this.refreshClueItemState();
        this.refreshContainItemState();
        this.checkInsertOver();
    }

    void checkCanInsert(string code, Vector2 pos)
    {
        float minDis = -1;
        int minDisContainCode = -1;
        foreach (KeyValuePair<int, ClueCombineContainView> item in itemViewMap)
        {
            string cotainCode = null;
            combineMap.TryGetValue(item.Key, out cotainCode);
            if (String.IsNullOrEmpty(cotainCode))
            {
                ClueCombineContainView containView = item.Value;
                if (containView != null)
                {
                    Vector3 worldPosition3D = containView.gameObject.transform.position;
                    Vector2 target = new Vector2(worldPosition3D.x, worldPosition3D.y);
                    float distanceThreshold = 100f;
                    float dis = Vector2.Distance(target, pos);
                    bool isClose = dis < distanceThreshold;
                    if (isClose)
                    {
                        if (minDis > 0)
                        {
                            if (dis < minDis)
                            {
                                minDis = dis;
                                minDisContainCode = item.Key;
                            }
                        }
                        else
                        {
                            minDis = dis;
                            minDisContainCode = item.Key;
                        }
                    }

                }
            }
        }

        if (minDisContainCode >= 0)
        {
            combineMap.Add(minDisContainCode, code);
        }
    }

    void checkInsertOver()
    {
        bool result = false;
        if (combineMap.Count == rightcombineList.Count)
        {
            HashSet<string> rightSet = new HashSet<string>(rightcombineList);
            HashSet<string> combineSet = new HashSet<string>(combineMap.Values.ToList());

            result = rightSet.SetEquals(combineSet);
            if (!result)
            {
                combineMap.Clear();
                this.refreshClueItemState();
                this.refreshContainItemState();
            }
        }


        if (result)
        {
            this.right_node.SetActive(result);
            this.layout_contain.gameObject.SetActive(!result);
            if (this.mergeClueConfig != null)
            {
                var config = ConfigController.Instance.GetClueItemConfig(this.mergeClueConfig?.completeClue);
                CommonUtils.updateText(config?.description ?? "", this.rightTxt);
            }
            Debug.Log("组合已完成");
            UIController.Instance.GetItemTip(new List<string>(){ mergeClueConfig.completeClue });
            GameDataProxy.Instance.finishedClueCombine.Add(mergeClueConfig.ID);
            gameObject.SetActive(false);
            GameLineNode node = new GameLineNode();
            node.type = GameNodeType.Puzzle;
            node.ID = mergeClueConfig.ID;
            MessageManager.Instance.Send(MessageDefine.GameLineNodeDone, new MessageData(node));
        }
    }

    void refreshClueItemState()
    {
        bool allComplete = true;
        List<string> allUseClueList = combineMap.Values.ToList()??new List<string>();
        foreach (KeyValuePair<string, ClueCombineItemView> item in clueItemViewMap)
        {
            item.Value.refreshContainState(!allUseClueList.Contains(item.Key));
        }
        if (allComplete)
        {
            //todo
        }
    }

    void refreshContainItemState()
    {
        foreach (KeyValuePair<int, ClueCombineContainView> item in itemViewMap)
        {
            string useCode = null;
            combineMap.TryGetValue(item.Key,out useCode);

            item.Value.updateView(useCode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
