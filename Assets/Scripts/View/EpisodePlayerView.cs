using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EpisodePlayerView : MonoBehaviour
{
    public enum EpisodePlayerStatus
    {
        Stop,  // dialogQueue为空，view关闭
        Typing,  // 打字机播放中
        Pause,  // 打字机播放完，dialogQueue不为空
    }

    public Image leftImg;
    public Image rightImg;
    public Text content;
    public HorizontalLayoutGroup horizontalChoiceLayout;
    public VerticalLayoutGroup verticalChoiceLayout;
    public Button nextBtn;
    public GameObject choicePartViewPrefab;

    protected Queue<DialogConfig> dialogQueue = new Queue<DialogConfig>();
    protected EpisodePlayerStatus curStatus;
    protected string targetText;
    protected float typeTimer = 0;
    protected bool useTypeEffect = true;
    protected int typeSpeed = 0;
    protected List<ChoiceConfig> choices = new List<ChoiceConfig>();
    protected List<DialogChoicePartView> choiceView = new List<DialogChoicePartView>();

    void Start()
    {
        this.gameObject?.SetActive(false);
        nextBtn?.onClick.AddListener(onNextBtnClick);
        ChangeStatus(EpisodePlayerStatus.Stop);
        nextBtn.interactable = false;
    }

    void Update()
    {
        if (curStatus == EpisodePlayerStatus.Typing)
        {
            typeTimer += Time.deltaTime * typeSpeed;
            content.text = targetText.Substring(0, (int)typeTimer);
            Debug.Log(content);
            if (content.text == targetText)
            {
                ChangeStatus(EpisodePlayerStatus.Pause);
                if (choices?.Count > 0)
                {
                    ShowChoices();
                }
            }
        }
    }

    private void onNextBtnClick()
    {
        switch (curStatus)
        {
            case EpisodePlayerStatus.Stop:
                break;
            case EpisodePlayerStatus.Typing:
                typeSpeed = ConfigController.Instance.maxTypingSpeed;
                break;
            case EpisodePlayerStatus.Pause:
                GoNext();
                break;
        }
    }

    public virtual void UpdateView(DialogConfig dialog)
    {

    }

    public void PlayEpisode(int episodeID)
    {
        if (episodeID < 0)
        {
            return;
        }
        horizontalChoiceLayout.gameObject.SetActive(false);
        verticalChoiceLayout.gameObject.SetActive(false);
        nextBtn.interactable = true;
        var tempQueue = dialogQueue;
        var config = ConfigController.Instance;
        dialogQueue = new Queue<DialogConfig>();
        EpisodeConfig episode = config.GetEpisode(episodeID);
        foreach (var id in episode.dialogList)
        {
            DialogConfig dialog = config.GetDialog(id);
            if (dialog != null)
            {
                dialog.episodeType = episode.episodeType;
                dialogQueue.Enqueue(dialog);  // 中途插进来的先播
            }
        }
        // 把之前没播完的塞回来
        while (tempQueue.Count > 0)
        {
            dialogQueue.Enqueue(tempQueue.Dequeue());
        }
        GoNext();
    }

    public void GoNext()
    {
        if (dialogQueue.Count == 0)
        {
            ChangeStatus(EpisodePlayerStatus.Stop);
            this.gameObject.SetActive(false);
            return;
        }
        var configController = ConfigController.Instance;
        var dialog = dialogQueue.Dequeue();
        if (dialog != null)
        {
            if (dialog.choices.Count > 0)
            {
                // 出现选项
                foreach (var choiceID in dialog.choices)
                {
                    ChoiceConfig choice = configController.GetChoice(choiceID);
                    if (choice != null)
                    {
                        choice.episodeType = dialog.episodeType;
                        choices.Add(choice);
                    }
                }
            } else
            {
                choices = null;
            }
            if (useTypeEffect)
            {
                targetText = dialog.content;
                typeSpeed = configController.normalTypingSpeed;
                typeTimer = 0;
                ChangeStatus(EpisodePlayerStatus.Typing);
            }
            else
            {
                content.text = dialog.content;
                ChangeStatus(EpisodePlayerStatus.Pause);
            }
            UpdateView(dialog);
            GameDataProxy.Instance.historyDialog.Add(dialog);
        }
    }

    protected void ChangeStatus(EpisodePlayerStatus status)
    {
        curStatus = status;
    }

    protected void ShowChoices()
    {
        nextBtn.interactable = false;
        if (choiceView.Count > choices.Count)
        {
            for (int i = choices.Count; i < choiceView.Count; i++)
            {
                choiceView[i]?.gameObject?.SetActive(false);
            }
        }
        for (int i = 0; i < choices.Count; i++)
        {
            if (choiceView[i] == null)
            {
                GameObject gameObj = Instantiate(choicePartViewPrefab);
                choiceView[i] = gameObj.GetComponent<DialogChoicePartView>();
            }
            choiceView[i]?.UpdateView(choices[i]);
        }

    }
}
