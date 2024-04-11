using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogChoicePartView : MonoBehaviour
{
    public Text content;
    public Button clickBtn;

    private ChoiceConfig choiceConfig = null;
    private int triggerEpisodeID = -1;

    void Start()
    {
        clickBtn.onClick.AddListener(onClickBtnClick);
    }

    public void UpdateView(ChoiceConfig choice)
    {
        choiceConfig = choice;
        triggerEpisodeID = choice.triggerEpisodeID;
        this.gameObject.SetActive(true);
        content.text = choiceConfig.content;
    }

    private void onClickBtnClick()
    {
        if (choiceConfig?.episodeType == EpisodeType.Normal)
        {
            ControllerManager.Get().UIController?.episodePlayerView?.PlayEpisode(triggerEpisodeID);
        }
        else if (choiceConfig?.episodeType == EpisodeType.Phone)
        {
            ControllerManager.Get().UIController?.phoneEpisodePlayerView?.PlayEpisode(triggerEpisodeID);
        }
    }
}
