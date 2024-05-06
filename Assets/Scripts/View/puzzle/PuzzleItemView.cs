using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleItemView : MonoBehaviour
{
    //public Image pic;
    //DragNodeCompent dragNodeCom;

    Dictionary<JewelryType, InsertSiteView> insertViewMap=new Dictionary<JewelryType,InsertSiteView>();
    List<PuzzleCombineConfig> combineList = new List<PuzzleCombineConfig>();
    
    static Dictionary<JewelryType,GameObject> prefabMap=new Dictionary<JewelryType,GameObject>();

    Dictionary<int, GameObject> imageMap = new Dictionary<int, GameObject>();
    Dictionary<int, DragNodeCompent> dragNodeComMap = new Dictionary<int, DragNodeCompent>();

    public static GameObject getPrefab(JewelryType type)
    {
        int useType = (int)type;

        GameObject prefab = null;
        bool isExist = prefabMap.TryGetValue(type, out prefab);

        if (!isExist)
        {
            string resourceName = "Prefabs/UI/puzzle/JewelryView_"+ useType.ToString(); // 资源名称
            if(type == JewelryType.Human)
            {
                resourceName = "Prefabs/UI/puzzle/MainObjectView" ; // 资源名称
            }
            prefab = Resources.Load<GameObject>(resourceName);
            prefabMap.Add(type, prefab);
        }

        return prefab;
    }

    private void Awake()
    {
        this.initImage();
    }

    void initImage()
    {
        for(int i = 1; i < 5; i++)
        {
            GameObject imageObject = CommonUtils.findChildByName(transform, "image_"+i.ToString())?.gameObject;
            if (imageObject != null)
            {
                imageMap.Add(i, imageObject);

                DragNodeCompent dragCom = imageObject.AddComponent<DragNodeCompent>();
                dragCom.enabled = false;

                dragNodeComMap.Add(i, dragCom);
            }
        }
    }

    public void updateCanDrag(bool canDrag,int code)
    {
        foreach (KeyValuePair<int, DragNodeCompent> item in dragNodeComMap)
        {
            bool canUse = canDrag && ((code != 0 && code == item.Key) || (code == 0));
            item.Value.enabled = canUse;
            if (!canUse)
            {
                GameObject imageNode =null;
                imageMap.TryGetValue(item.Key, out imageNode);
                if(imageNode != null)
                {
                    //imageNode.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    public void updateView(PuzzleItemConfig config,bool isSub
        , Action<GameObject, int> dragStartCallback
        , Action<Vector2> dragMoveCallback
        , Action<Vector2,int> dragOverCallback)
    {
        //Debug.Log("updateView");
        //CommonUtils.updateImage(config?.url, pic);
        if (isSub)
        {
            foreach (KeyValuePair<int, DragNodeCompent> item in dragNodeComMap)
            {
                GameObject imageNode = null;
                bool isExist =  imageMap.TryGetValue(item.Key, out imageNode);
                if (isExist)
                {
                    Image pic = imageNode.GetComponent<Image>();
                    GameObject explainTxt = CommonUtils.findChildByName(pic.gameObject.transform,"explainTxt")?.gameObject;
                    item.Value?.init(dragStartCallback, dragMoveCallback, dragOverCallback, item.Key, pic);

                    if (explainTxt != null)
                    {
                        explainTxt.SetActive(false);
                        item.Value?.initPointer(() =>
                        {
                            if (explainTxt)
                            {
                                explainTxt.SetActive(true);
                            }
                        }, () =>
                        {
                            if (explainTxt)
                            {
                                explainTxt.SetActive(false);
                            }
                        });
                    }

                }

            }
        }
        else
        {
            foreach (KeyValuePair<int, DragNodeCompent> item in dragNodeComMap)
            {
                item.Value.enabled = false;
            }
        }
    }

    public void refreshContainState(int useCode)
    {
        foreach (KeyValuePair<int, GameObject> item in imageMap)
        {
            bool isUsed = useCode == item.Key;
            item.Value.SetActive(!isUsed);

            item.Value.GetComponent<DragNodeCompent>().enabled = !isUsed;
        }
    }

    public void updateInsertView()
    {
        combineList = GameDataProxy.Instance.puzzleCombineConfigs;
        if (this.combineList.Count > 0)
        {
            GameObject prefab = InsertSiteView.getPrefab();
            foreach (PuzzleCombineConfig itemConfig in this.combineList)
            {
                JewelryType viewID = itemConfig.jewelryType;
                InsertSiteView itemView = null;
                bool isExist = insertViewMap.TryGetValue(viewID, out itemView);
                if (!isExist)
                {
                    Transform content = transform;
                    itemView = CommonUtils.CreateViewByType<InsertSiteView>(prefab, content);

                    itemView.gameObject.transform.localPosition = new Vector2(itemConfig.xPos, itemConfig.yPos);

                    if(viewID == JewelryType.Watch)
                    {
                        // 获取当前的旋转值
                        Vector3 currentRotation = itemView.gameObject.transform.rotation.eulerAngles;
                        // 设置新的Z轴旋转值
                        currentRotation.z = 25;
                        // 应用新的旋转值
                        itemView.gameObject.transform.rotation = Quaternion.Euler(currentRotation);
                    }

                    insertViewMap[viewID] = itemView;
                }
                bool complete = GameDataProxy.Instance.checkJewelryComplete(viewID);
                int useCode = 0;
                GameDataProxy.Instance.insertjewelryMap.TryGetValue(itemConfig.jewelryType,out useCode);

                string url = null;
                if(complete && useCode > 0)
                {
                    url = ConfigController.Instance.getJewelryUrl(viewID, useCode);
                }
                itemView?.updateView(url);
                //string url = complete?:null
                //?.updateView()
            }
        }
    }
}
