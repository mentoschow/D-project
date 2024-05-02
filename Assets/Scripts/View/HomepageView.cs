using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomepageView : MonoSingleton<HomepageView>
{
    [SerializeField]
    private Button gameStartBtn;
    [SerializeField]
    private Button quitBtn;
    [SerializeField]
    private Button staffBtn;
    [SerializeField]
    private GameObject staffArea;

    void Start()
    {
        gameStartBtn?.onClick.AddListener(GameStart);
        quitBtn?.onClick.AddListener(QuitGame);
        staffBtn?.onClick.AddListener(ShowStaff);
        staffArea.SetActive(false);
    }

    private void GameStart()
    {
        GameController.Instance.GameStart();
    }

    private void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void ShowStaff()
    {
        var active = staffArea.activeInHierarchy;
        staffArea.SetActive(!active);
    }
}
