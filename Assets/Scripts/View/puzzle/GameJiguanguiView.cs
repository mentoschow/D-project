using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameJiguanguiView : MonoBehaviour
{
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Button keyBtn;
    [SerializeField]
    private Button topBtn;
    [SerializeField]
    private Button book1Btn;
    [SerializeField]
    private Button book2Btn;
    [SerializeField]
    private RectTransform childViewNode;
    [SerializeField]
    private GameObject puzzleBeforeObj;
    [SerializeField]
    private GameObject puzzleObj;

    private PuzzleBeforeView puzzleBeforeView;
    private PuzzleView puzzleView;

    void Start()
    {
        closeBtn?.onClick.AddListener(Close);
        keyBtn?.onClick.AddListener(OnKeyClick);
        topBtn?.onClick.AddListener(OnTopClick);
        book1Btn?.onClick.AddListener(OnBook1Click);
        book2Btn?.onClick.AddListener(OnBook2Click);
    }

    private void OnEnable()
    {
        puzzleView?.gameObject.SetActive(false);
        puzzleBeforeView?.gameObject.SetActive(false);
    }

    private void OnKeyClick()
    {
        UIController.Instance.PlayEpisode("SQ01_030_020");
    }

    private void OnBook1Click()
    {
        UIController.Instance.PlayEpisode("SQ01_030_050");
    }

    private void OnBook2Click()
    {
        UIController.Instance.PlayEpisode("SQ01_030_052");
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnTopClick()
    {
        if (GameDataProxy.Instance.canPlayJiguangui)
        {
            if (puzzleView == null)
            {
                puzzleView = BaseFunction.CreateView<PuzzleView>(puzzleObj, childViewNode);
            }
            puzzleView?.gameObject.SetActive(true);
            var config = ConfigController.Instance.puzzleConfig;
            puzzleView?.updateView(config);
        }
        else
        {
            if (puzzleBeforeView == null)
            {
                puzzleBeforeView = BaseFunction.CreateView<PuzzleBeforeView>(puzzleBeforeObj, childViewNode);
            }
            puzzleBeforeView?.gameObject.SetActive(true);
        }
    }
}
