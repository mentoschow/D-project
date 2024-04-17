using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogChoicePartView : MonoBehaviour
{
    public Text content;
    public Button clickBtn;

    private ChoiceConfig choiceConfig = null;
    private string triggerEpisodeID = "";

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
        
    }
}
