using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickPointPartView : MonoBehaviour
{
    public int pointID;
    public List<ClickPointConfig> configs = new List<ClickPointConfig>();

    private bool isFirstClick = true;
    private Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        
    }

    private void OnButtonClick()
    {
        isFirstClick = false;
    }
}
