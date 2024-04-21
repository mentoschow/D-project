using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EpisodePlayerView : MonoBehaviour
{
    public enum EpisodePlayerStatus
    {
        Stop,  // dialogQueue为空，view关闭
        Typing,  // 打字机播放中
        Pause,  // 打字机播放完，dialogQueue不为空
    }

    [SerializeField]
    private int normalTypingSpeed = 10;
    [SerializeField]
    private int maxTypingSpeed = 20;
    [SerializeField]
    private GameObject choicePartViewPrefab;
    [SerializeField]
    private Sprite boySp;
    [SerializeField]
    private Sprite girlSp;
    [SerializeField]
    private GameObject leftDialogObj;
    [SerializeField]
    private GameObject rightDialogObj;
    [SerializeField]
    private Button nextBtn;
    [SerializeField]
    private GameObject nextArrow;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Image roleImg;
    [SerializeField]
    private Text content;
    [SerializeField]
    private HorizontalLayoutGroup horizontalChoiceLayout;
    [SerializeField]
    private VerticalLayoutGroup verticalChoiceLayout;
    [SerializeField]
    private bool useTypeEffect = true;
    [SerializeField]
    private ScrollRect phoneScrollRect;

    private Queue<DialogConfig> dialogQueue = new Queue<DialogConfig>();
    private EpisodePlayerStatus curStatus;
    private string targetText;
    private float typeTimer = 0;
    private int typeSpeed = 0;
    private List<ChoiceConfig> choices = new List<ChoiceConfig>();
    private List<DialogChoicePartView> choiceView = new List<DialogChoicePartView>();
    private string curEpisodeID = "";
    private EpisodeType curType;
    private List<PhoneEpisodePartView> phoneDialogList = new List<PhoneEpisodePartView>();

    void Awake()
    {
        if (nextBtn != null)
        {
            nextBtn.onClick.AddListener(onNextBtnClick);
            nextBtn.interactable = false;
        }
        if (phoneScrollRect != null)
        {
            
        }
        ChangeStatus(EpisodePlayerStatus.Stop);
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
                typeSpeed = maxTypingSpeed;
                break;
            case EpisodePlayerStatus.Pause:
                GoNext();
                break;
        }
    }

    public virtual void UpdateNormalView(DialogConfig dialog)
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
            typeSpeed = normalTypingSpeed;
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
        switch (dialog.roleType)
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
        var tempQueue = dialogQueue;
        var config = ConfigController.Instance;
        dialogQueue = new Queue<DialogConfig>();
        EpisodeConfig episode = config.GetEpisodeConfig(episodeID);
        curType = episode.episodeType;
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
            gameObject.SetActive(false);
            ChangeStatus(EpisodePlayerStatus.Stop);
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

    private void UpdateView(DialogConfig dialog)
    {
        if (curType == EpisodeType.Normal) {
            UpdateNormalView(dialog);
        }
        else if (curType == EpisodeType.Phone)
        {
            UpdatePhoneView(dialog);
        }
        if (nextBtn != null)
        {
            nextBtn.interactable = true;
        }
    }

    private void UpdatePhoneView(DialogConfig dialog)
    {
        GameObject obj = null;
        Sprite icon = null;
        string name = "";
        switch (dialog.roleType)
        {
            case RoleType.MainRoleGirl:
                obj = rightDialogObj;
                icon = girlSp;
                name = "女主角";
                break;
            case RoleType.MainRoleBoy:
                obj = leftDialogObj;
                icon = boySp;
                name = "男主角";
                break;
        }
        var view = BaseFunction.CreateView<PhoneEpisodePartView>(obj, transform);
        if (view != null)
        {
            ChangeStatus(EpisodePlayerStatus.Pause);
            view.UpdateView(icon, name, dialog.content);
            phoneDialogList.Add(view);
        }
        Invoke("ScrollToEnd", 0.1f);
    }

    private void ScrollToEnd()
    {
        var startValue = phoneScrollRect.verticalScrollbar.value;
        DOTween.To(value =>
        {
            phoneScrollRect.verticalScrollbar.value = value;
        }, startValue, 0, 0.3f);
    }
}
