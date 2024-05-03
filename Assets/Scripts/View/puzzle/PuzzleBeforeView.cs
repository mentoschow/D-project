using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBeforeView : MonoBehaviour
{
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Button photoBtn;
    [SerializeField]
    private List<Button> jewelryBtns;
    void Start()
    {
        closeBtn?.onClick.AddListener(Close);
        photoBtn?.onClick.AddListener(OnPhotoClick);
        foreach (var button in jewelryBtns)
        {
            button?.onClick.AddListener(OnJewelryClick);
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnPhotoClick()
    {
        UIController.Instance.PlayEpisode("SQ01_030_030");
    }

    private void OnJewelryClick()
    {
        UIController.Instance.PlayEpisode("SQ01_030_040");
    }
}
