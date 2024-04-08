using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomepageView : MonoBehaviour
{
    public Button gameStartBtn;

    void Start()
    {
        gameStartBtn = transform.Find("gameStartBtn")?.GetComponent<Button>();
        gameStartBtn.onClick.AddListener(onGameStartBtnClick);
    }

    void Update()
    {
        
    }

    private void onGameStartBtnClick()
    {
        GameController gameController = ControllerManager.GetManager().gameController;
        gameController?.GameStart();
    }
}
