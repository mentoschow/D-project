using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPointPanelView : MonoBehaviour
{
    public int puzzleID;

    private List<ClickPointPartView> childView;
    
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<ClickPointPartView>();
            if (child != null)
            {
                childView.Add(child);
            }
        }
    }
}
