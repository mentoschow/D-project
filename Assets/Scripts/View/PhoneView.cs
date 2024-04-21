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

    void Start()
    {
        itemBtn?.onClick.AddListener(OnItemBtnClick);
        episodeBtn?.onClick.AddListener(OnEpisodeBtnClick);
        closeBtn?.onClick.AddListener(Close);
    }

    public void ShowPhone()
    {
        gameObject.SetActive(true);
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
        phoneEpisodePlayer.gameObject.SetActive(true);
    }

    private void ShowItemView()
    {

    }

    private void OnItemBtnClick()
    {
        ShowItemView();
    }

    private void OnEpisodeBtnClick()
    {
        ShowEpisodeView();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
