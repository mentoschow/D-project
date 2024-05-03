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
        MergeClueConfig clueConfig = new MergeClueConfig();
        clueConfig.ID = "MS01_020_telegraph";
        clueConfig.correctClueList = new List<string> { "CUE_0020_review_telegraph", "CUE_0030_review_telegraphoffice", "CUE_0040_review_college" };
        clueConfig.prepareClueList = new List<string> { "CUE_0005_message_library", "CUE_0010_message_miss", "CUE_0020_review_telegraph", "CUE_0030_review_telegraphoffice", "CUE_0040_review_college" };
        clueConfig.completeClue = "CUE_0050_reason_telegraph";

        UIController.Instance.showClueCombineView(clueConfig);
        gameObject.SetActive(false);
    }

    void onPuzzleBtnClick() {
        UIController.Instance.showJiguanguiView();
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
