using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestControlView : MonoSingleton<TestControlView>
{
    public Button puzzleBtn;
    public Button closeBtn;
    // Start is called before the first frame update

    private void Awake()
    {
        puzzleBtn.onClick.AddListener(onPuzzleBtnClick);
        closeBtn.onClick.AddListener(onCloseBtnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onPuzzleBtnClick() {
        UIController.Instance.showPuzzleView();
        gameObject.SetActive(false);
    }
    void onCloseBtnClick()
    {
        gameObject.SetActive(false);
    }
}
