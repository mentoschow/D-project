using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomepageView : MonoSingleton<HomepageView>
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
        GameController.Instance.GameStart();
    }
}
