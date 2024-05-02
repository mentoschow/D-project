using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ClueCombineView : MonoSingleton<ClueCombineView>
{
    // Start is called before the first frame update
    public GameObject layout_right;
    public GameObject layout_left;
    public GameObject layout_contain;
    public Button closeBtn;
    public GameObject curDragAttachNode;

    Dictionary<int, ClueCombineContainView> itemViewMap = new Dictionary<int, ClueCombineContainView>();

    Dictionary<int, string> combineMap = new Dictionary<int, string>();
    Dictionary<int, string> rightcombineMap = new Dictionary<int, string>();

    int rightCout = 0;

    Dictionary<string, ClueCombineItemView> clueItemViewMap = new Dictionary<string, ClueCombineItemView>();

    string  moveItemCode = "";
    GameObject curDragNode = null;

    static GameObject ViewPrefab;
    public static GameObject getPrefab()
    {
        if (!ClueCombineView.ViewPrefab)
        {
            string resourceName = "Prefabs/UI/ClueCombine/ClueCombineView"; // 资源名称
            ClueCombineView.ViewPrefab = Resources.Load<GameObject>(resourceName);
        }

        return ClueCombineView.ViewPrefab;
    }

    private void Awake()
    {
        layout_right = CommonUtils.findChildByName(transform, "layout_right").gameObject;
        layout_left = CommonUtils.findChildByName(transform, "layout_left").gameObject;
        layout_contain = CommonUtils.findChildByName(transform, "layout_left").gameObject;
        curDragAttachNode = CommonUtils.findChildByName(transform, "curDragAttachNode").gameObject;

        closeBtn = CommonUtils.findChildByName(transform, "closeBtn").gameObject.GetComponent<UnityEngine.UI.Button>();
        closeBtn?.onClick.AddListener(onCloseBtnClick);
        this.initAttachNodeMap();
    }

    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }

    void initAttachNodeMap()
    {
        {
            for (int i = 0; i < 3; i++)
            {
                ClueCombineContainView itemView = CommonUtils.CreateViewByType<ClueCombineContainView>(ClueCombineContainView.getPrefab(), layout_contain.transform);
                itemViewMap.Add(i, itemView);
            }
        }
    }

    void resetView()
    {
        rightcombineMap = new Dictionary<int, string>();
        combineMap = new Dictionary<int, string>();

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

    public void updateView(List<string> clueList, List<string> rightCombineList)
    {
        this.rightCout = rightCombineList.Count;
        this.resetView();

        for (int i = 0; i < rightCout; i++)
        {
            string rightCode = rightCombineList[i];
            rightcombineMap.Add(i, rightCode);
        }
        this.updateClue(clueList);
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
    }

    void checkCanInsert(string code, Vector2 pos)
    {
        Dictionary<int, float> disMap = new Dictionary<int, float>();
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
                    float distanceThreshold = 60f;
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

        if (minDisContainCode > 0)
        {
            combineMap.Add(minDisContainCode, code);
        }
    }

    void checkInsertOver()
    {
        bool result = false;
        if (combineMap.Count == rightcombineMap.Count)
        {
            result = combineMap.Equals(rightcombineMap);
            if (!result)
            {
                combineMap.Clear();
            }
        }
        if (result)
        {
            Debug.Log("组合已完成");
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
