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
    [SerializeField]
    private GameObject gameStartPage;
    [SerializeField]
    private GameObject gameOverPage;
    [SerializeField]
    private Button gameOverQuitBtn;

    void Start()
    {
        gameStartBtn?.onClick.AddListener(GameStart);
        quitBtn?.onClick.AddListener(QuitGame);
        staffBtn?.onClick.AddListener(ShowStaff);
        gameOverQuitBtn?.onClick.AddListener(QuitGame);
        gameOverPage?.SetActive(false);
        gameStartPage?.SetActive(true);
        staffArea.SetActive(false);
    }

    private void GameStart()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.GameStartButton);
        GameController.Instance.GameStart();
    }

    private void QuitGame()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.NormalButton);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void ShowStaff()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.NormalButton);
        var active = staffArea.activeInHierarchy;
        staffArea.SetActive(!active);
    }

    public void ShowGameOver()
    {
        gameStartPage.SetActive(false);
        gameStartBtn?.gameObject.SetActive(false);
        staffArea?.SetActive(false);
        gameOverPage.SetActive(true);
    }
}
