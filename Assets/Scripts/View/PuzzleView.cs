using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleView : MonoSingleton<PuzzleView>
{
    // Start is called before the first frame update
    public GameObject mainObject;
    public GameObject subObject;
    public HorizontalLayoutGroup layout;

    Dictionary<string, PuzzleItemView> itemViewMap = new Dictionary<string, PuzzleItemView>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateView(List<PuzzleItemConfig> configList)
    {
        int len = configList?.Count??0;
        if (len>0)
        {
            foreach (PuzzleItemConfig config in configList)
            {
                GameObject prefab = config.combineList.Count>0 ? mainObject : subObject;
                string viewID = config?.itemID ?? "";
                PuzzleItemView itemView = itemViewMap[viewID];
                if(itemView==null)
                {
                    itemView = UIController.Instance.CreateView<PuzzleItemView>(prefab, layout.transform);
                    itemViewMap[viewID] = itemView;
                }
                itemView?.updateView(config);
            }
        }
    }
}
