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
    public GameObject subObject;
    public HorizontalLayoutGroup layout;

    public Button closeBtn;

    Dictionary<string, PuzzleItemView> itemViewMap = new Dictionary<string, PuzzleItemView>();

    private void Awake()
    {
        closeBtn.onClick.AddListener(onCloseBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView(PuzzleConfig config)
    {
        List<PuzzleItemConfig> configList = config?.itemConfigList ?? new List<PuzzleItemConfig>();
        int len = configList?.Count??0;
        if (len>0)
        {
            foreach (PuzzleItemConfig itemConfig in configList)
            {
                GameObject prefab = itemConfig.combineList.Count>0 ? mainObject : subObject;
                string viewID = itemConfig?.itemID ?? "";

                PuzzleItemView itemView;
                bool isExist = itemViewMap.TryGetValue(viewID,out itemView);
                if(!isExist)
                {
                    Transform content = layout.transform;
                    // ��"Content"������ʵ����һ���µĽڵ�
                    GameObject newNode = Instantiate(prefab, content);
                    itemView = newNode.AddComponent<PuzzleItemView>();
                    //itemView = newNode.GetComponent<PuzzleItemView>();
           
                    // ����������λ�ã���ѡ��
                    //RectTransform rt = itemView.GetComponent<RectTransform>();
                   // rt.anchoredPosition = new Vector2(0, 0); // ����Layout��������λ��

                    itemViewMap[viewID] = itemView;
                }
                itemView?.updateView(itemConfig);
            }


        }
    }

    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }
}
