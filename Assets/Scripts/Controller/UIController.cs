using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private HomepageView homepageView = null;
    private GameStageView gameStageView = null;
    private LoadingView loadingView = null;

    private void Awake()
    {
        homepageView = GetComponentInChildren<HomepageView>();
        gameStageView = GetComponentInChildren<GameStageView>();
        loadingView = GetComponentInChildren<LoadingView>();
    }

    void Start()
    {
        BaseFunction.UpdateActive(homepageView, true);
        BaseFunction.UpdateActive(gameStageView, false);
        BaseFunction.UpdateActive(loadingView, false);

    }

    void Update()
    {
        
    }

    public void ChangeStage()
    {
        Debug.Log("游戏开始了");
    }
}
