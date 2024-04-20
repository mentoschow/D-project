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

    public GameObject choicePartViewPrefab;
    public Sprite boySp;
    public Sprite girlSp;

    protected Image roleImg;
    protected Text content;
    protected HorizontalLayoutGroup horizontalChoiceLayout;
    protected VerticalLayoutGroup verticalChoiceLayout;
    protected Button nextBtn;
    protected GameObject nextArrow;
    protected Text nameText;

    protected Queue<DialogConfig> dialogQueue = new Queue<DialogConfig>();
    protected EpisodePlayerStatus curStatus;
    protected string targetText;
    protected float typeTimer = 0;
    protected bool useTypeEffect = true;
    protected int typeSpeed = 0;
    protected List<ChoiceConfig> choices = new List<ChoiceConfig>();
    protected List<DialogChoicePartView> choiceView = new List<DialogChoicePartView>();
    protected string curEpisodeID = "";

    void Awake()
    {
        roleImg = transform.Find("roleImg")?.GetComponent<Image>();
        content = transform.Find("textArea")?.Find("contentText")?.GetComponent<Text>();
        nextBtn = transform.Find("nextBtn")?.GetComponent<Button>();
        nextArrow = transform.Find("textArea")?.Find("arrow")?.gameObject;
        nameText = transform.Find("textArea")?.Find("nameText")?.GetComponent<Text>();

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
                nextArrow?.SetActive(true);
                if (choices?.Count > 0)
                {
                    ShowChoices();
                }
            }
            else
            {
                nextArrow?.SetActive(false);
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
        var configController = ConfigController.Instance;
        // 选项
        if (dialog.choices.Count > 0)
        {
            foreach (var choiceID in dialog.choices)
            {
                ChoiceConfig choice = configController.GetChoiceConfig(choiceID);
                if (choice != null)
                {
                    choices.Add(choice);
                }
            }
        }
        else
        {
            choices = null;
        }
        // 打字机
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
        // 获得道具
        if (dialog.getItemID.Count > 0)
        {
            UIController.Instance.GetItemTip(dialog.getItemID, transform);
        }
        // 中间的图片
        if (!string.IsNullOrEmpty(dialog.showImgUrl))
        {

        }
        // 立绘、名字
        switch (dialog.character)
        {
            case RoleType.MainRoleGirl:
                nameText.text = "女主角";
                roleImg.sprite = girlSp;
                break;
            case RoleType.MainRoleBoy:
                nameText.text = "男主角";
                roleImg.sprite = boySp;
                break;
            default:
                nameText.text = "";
                roleImg.sprite = null;
                break;
        }
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
        if (dialogQueue.Count == 0)  // 结束了
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
        var dialog = dialogQueue.Dequeue();
        if (dialog != null)
        {
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
