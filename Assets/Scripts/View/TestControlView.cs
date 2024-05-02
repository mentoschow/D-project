using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestControlView : MonoSingleton<TestControlView>
{
    public Button puzzleBtn;
    public Button mimaeBtn;
    public Button clueMergeBtn;
    public Button closeBtn;
    // Start is called before the first frame update

    private void Awake()
    {
        puzzleBtn.onClick.AddListener(onPuzzleBtnClick);
        mimaeBtn.onClick.AddListener(onMimaBtnClick);
        closeBtn.onClick.AddListener(onCloseBtnClick);
        clueMergeBtn.onClick.AddListener(onClueMergeBtnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void onClueMergeBtnClick()
    {
        UIController.Instance.showClueCombineView(null);
        gameObject.SetActive(false);
    }

    void onPuzzleBtnClick() {
        UIController.Instance.showPuzzleView();
        gameObject.SetActive(false);
    }
    void onMimaBtnClick()
    {
        UIController.Instance.showMimaView();
        gameObject.SetActive(false);
    }
    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }
}
