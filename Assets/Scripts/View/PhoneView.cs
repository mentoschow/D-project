using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneView : MonoBehaviour
{
    [SerializeField]
    private Button itemBtn;
    [SerializeField]
    private Button episodeBtn;
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private EpisodePlayerView phoneEpisodePlayer;
    [SerializeField]
    private GameObject phoneHomepage;
    [SerializeField]
    private GameObject episodePage;

    void Start()
    {
        itemBtn?.onClick.AddListener(ShowItemView);
        episodeBtn?.onClick.AddListener(ShowEpisodeView);
        closeBtn?.onClick.AddListener(Close);
    }

    public void ShowPhone()
    {
        gameObject.SetActive(true);
        phoneHomepage?.SetActive(true);
        episodePage.SetActive(false);
        closeBtn.interactable = true;
    }

    public void PlayPhoneEpisode(string id)
    {
        gameObject.SetActive(true);
        closeBtn.interactable = false;
        ShowEpisodeView();
        phoneEpisodePlayer.PlayEpisode(id);
    }

    private void ShowEpisodeView()
    {
        episodePage.SetActive(true);
        phoneEpisodePlayer.gameObject.SetActive(true);
    }

    private void ShowItemView()
    {

    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
