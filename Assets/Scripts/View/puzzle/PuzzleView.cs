using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PuzzleView : MonoSingleton<PuzzleView>
{
    // Start is called before the first frame update
    public GameObject mainObject;
    [SerializeField]
    private Button finishBtn;
    PuzzleConfig useConfig;

    public GameObject mainObjectNode;
    public Button closeBtn;

    Dictionary<JewelryType, PuzzleItemView> itemViewMap = new Dictionary<JewelryType, PuzzleItemView>();
    PuzzleItemView mainObjectView;


    Dictionary<int, GameObject> itemAttachNodeMap = new Dictionary<int, GameObject>();

    JewelryType moveItemType = JewelryType.Not;
    int moveItemCode = 0;
    GameObject curDragNode = null;

    public GameObject curDragAttachNode;

    private void Awake()
    {
        closeBtn.onClick.AddListener(onCloseBtnClick);
        finishBtn?.onClick.AddListener(OnFinishClick);
        initAttachNodeMap();
#if UNITY_EDITOR
        finishBtn.gameObject.SetActive(true);
#else
        finishBtn.gameObject.SetActive(false);
#endif
    }

    void initAttachNodeMap()
    {
        {
            for (int i = 1; i <= 5; i++)
            {
                GameObject itemAttachNode = CommonUtils.findChildByName(transform, "GameObject_" + i.ToString()).gameObject;
                itemAttachNodeMap.Add(i, itemAttachNode);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView(PuzzleConfig config)
    {
        this.useConfig = config;

        List<PuzzleItemConfig> configList = config?.itemConfigList ?? new List<PuzzleItemConfig>();
        int len = configList?.Count??0;
        if (len>0)
        {
            foreach (PuzzleItemConfig itemConfig in configList)
            {
                GameObject prefab = PuzzleItemView.getPrefab(itemConfig.jewelryType);
                JewelryType viewID = itemConfig?.jewelryType ?? 0;
                if (prefab != null)
                {
                    bool isSub = GameDataProxy.Instance.getCombineConfig(viewID) != null;
                    Action<GameObject, int> dragStartCallback = null;
                    Action<Vector2> dragMoveCallback = null;
                    Action<Vector2, int> dragOverCallback = null;


                    PuzzleItemView itemView = null;
                    if (isSub)
                    {
                        bool isExist = itemViewMap.TryGetValue(viewID, out itemView);
                        if (!isExist)
                        {
                            GameObject layout = null;
                            itemAttachNodeMap.TryGetValue((int)viewID, out layout);

                            Transform content = layout.transform;
                            // 在"Content"对象下实例化一个新的节点
                            GameObject newNode = Instantiate(prefab, content);
                            itemView = newNode.AddComponent<PuzzleItemView>();

                            itemViewMap[viewID] = itemView;
                        }
                        dragStartCallback = (GameObject dragNode, int code) => {
                            this.moveItemType = viewID;
                            this.moveItemCode = code;
                            if (this.curDragNode)
                            {
                                Destroy(this.curDragNode);
                            }
                            this.curDragNode = Instantiate(dragNode, curDragAttachNode.transform);
                            GameObject explainTxt = CommonUtils.findChildByName(this.curDragNode.transform, "explainTxt")?.gameObject;
                            if (explainTxt!=null)
                            {
                                explainTxt.SetActive(true);
                            }
                        };
                        dragMoveCallback = (Vector2 pos) =>
                        {
                            this.curDragNode.transform.position = pos;
                        };
                        dragOverCallback = (Vector2 inputPos, int code) =>
                        {
                            this.moveItemType = JewelryType.Not;
                            this.moveItemCode = 0;
                            if (this.curDragNode)
                            {
                                Destroy(this.curDragNode);
                            }

                            this.checkCanInsert(viewID, code, inputPos);
                            this.checkInsertOver();

                            this.refreshItemContainState();
                            this.refreshDragState();
                            this.mainObjectView?.updateInsertView();
                        };
                    }
                    else
                    {
                        itemView = this.mainObjectView;
                        if (!itemView)
                        {
                            Transform content = mainObjectNode.transform;
                            // 在"Content"对象下实例化一个新的节点
                            GameObject newNode = Instantiate(prefab, content);
                            itemView = newNode.AddComponent<PuzzleItemView>();
                            this.mainObjectView = itemView;

                            this.mainObjectView?.updateInsertView();
                        }
                    }
                    itemView?.updateView(itemConfig, isSub, dragStartCallback, dragMoveCallback, dragOverCallback);
                }
                else
                {
                    Debug.LogError("空的预制体"+ ((int)viewID).ToString());
                }
            }
            this.refreshItemContainState();

        }
    }

    void checkCanInsert(JewelryType type,int code,Vector2 pos)
    {
       bool canUse = !GameDataProxy.Instance.checkJewelryComplete(type);
        if(canUse)
        {
            bool canInsert = canUse;
            if (canInsert)
            {
                PuzzleCombineConfig config = GameDataProxy.Instance.getCombineConfig(type);

                if (mainObjectView)
                {
                    Vector3 worldPosition3D = mainObjectView.gameObject.transform.position;
                    Vector2 target = new Vector2(worldPosition3D.x + config.xPos, worldPosition3D.y + config.yPos);
                    float distanceThreshold = 60f;
                    bool isClose = Vector2.Distance(target, pos) < distanceThreshold;

                    if (isClose)
                    {
                        GameDataProxy.Instance.insertjewelryMap.Add(type, code);
                    }
                }
            }
        }
    }
    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }

    void refreshDragState()
    {
        foreach (KeyValuePair<JewelryType, PuzzleItemView> item in itemViewMap)
        {
            bool isSub = GameDataProxy.Instance.getCombineConfig(item.Key) != null;
            bool canDrag = (item.Key == this.moveItemType)||(this.moveItemType == JewelryType.Not);
            canDrag = canDrag && !GameDataProxy.Instance.checkJewelryComplete(item.Key);
  
            item.Value?.updateCanDrag(canDrag,this.moveItemCode);
        }
    }

    void refreshItemContainState()
    {
        bool allComplete = true;
        foreach (KeyValuePair<JewelryType, PuzzleItemView> item in itemViewMap)
        {
            PuzzleCombineConfig config = GameDataProxy.Instance.getCombineConfig(item.Key);
            bool isSub = config != null;
            if (isSub)
            {
                bool isComplete = GameDataProxy.Instance.checkJewelryComplete(item.Key);
                if (!isComplete)
                {
                    allComplete = false;
                }
                int useCode = 0;
                GameDataProxy.Instance.insertjewelryMap.TryGetValue(item.Key, out useCode);
                item.Value?.refreshContainState(useCode);
            }
        }
        if (allComplete)
        {
            //todo
        }
    }

    void checkInsertOver(bool over = false)
    {
        bool isOver = GameDataProxy.Instance.checkInsertOver();
        if (over)
        {
            isOver = true;
        }
        if (isOver)
        {
            Debug.Log("装饰已完成");
            gameObject.SetActive(false);
            SceneController.Instance.OnJewelryPuzzleDone();
        }
    }

    private void OnFinishClick()
    {
        checkInsertOver(true);
    }
}
