using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EpisodePlayerView : MonoSingleton<EpisodePlayerView>
{
    public enum EpisodePlayerStatus
    {
        Stop,  // dialogQueueΪ�գ�view�ر�
        Typing,  // ���ֻ�������
        Pause,  // ���ֻ������꣬dialogQueue��Ϊ��
    }

    public Image leftImg;
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
    protected string curEpisodeID = "";

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

    public void PlayEpisode(string episodeID)
    {
        if (episodeID == "" || episodeID == null)
        {
            return;
        }
        if (curEpisodeID == "")
        {
            curEpisodeID = episodeID;
        }
        //horizontalChoiceLayout.gameObject.SetActive(false);
        //verticalChoiceLayout.gameObject.SetActive(false);
        nextBtn.interactable = true;
        var tempQueue = dialogQueue;
        var config = ConfigController.Instance;
        dialogQueue = new Queue<DialogConfig>();
        EpisodeConfig episode = config.GetEpisodeConfig(episodeID);
        foreach (var id in episode.dialogList)
        {
            DialogConfig dialog = config.GetDialogConfig(id);
            if (dialog != null)
            {
                dialogQueue.Enqueue(dialog);  // ��;��������Ȳ�
            }
        }
        // ��֮ǰû�����������
        while (tempQueue.Count > 0)
        {
            dialogQueue.Enqueue(tempQueue.Dequeue());
        }
        GoNext();
    }

    public void GoNext()
    {
        if (dialogQueue.Count == 0)  // ������
        {
            ChangeStatus(EpisodePlayerStatus.Stop);
            this.gameObject.SetActive(false);
            GameLineNode node = new GameLineNode();
            node.type = GameNodeType.NormalEpisode;
            node.ID = curEpisodeID;
            MessageManager.Instance.Send(MessageDefine.PlayEpisodeDone, new MessageData(node));
            curEpisodeID = "";
            return;
        }
        var configController = ConfigController.Instance;
        var dialog = dialogQueue.Dequeue();
        if (dialog != null)
        {
            if (dialog.choices.Count > 0)
            {
                // ����ѡ��
                foreach (var choiceID in dialog.choices)
                {
                    ChoiceConfig choice = configController.GetChoiceConfig(choiceID);
                    if (choice != null)
                    {
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
