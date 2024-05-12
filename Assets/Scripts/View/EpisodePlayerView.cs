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
    private GameObject leftDialogObj;
    [SerializeField]
    private GameObject rightDialogObj;
    [SerializeField]
    public Button nextBtn;
    [SerializeField]
    private GameObject nextArrow;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Image roleImg;
    [SerializeField]
    private Image middleImg;
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
    [SerializeField]
    private Transform contentNode;

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
            nextBtn.onClick.AddListener(OnNextBtnClick);
            nextBtn.gameObject.SetActive(false);
        }
        ChangeStatus(EpisodePlayerStatus.Stop);
    }

    void Update()
    {
        if (curStatus == EpisodePlayerStatus.Typing)
        {
            typeTimer += Time.deltaTime * typeSpeed;
            int length = (int)typeTimer;
            if (length > targetText.Length)
            {
                length = targetText.Length;
            }
            content.text = targetText.Substring(0, length);
            if (content.text == targetText)
            {
                ChangeStatus(EpisodePlayerStatus.Pause);
                AudioController.Instance.Stop();
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

    public void OnNextBtnClick()
    {
        AudioController.Instance.PlayAudioEffect(curType == EpisodeType.Normal ? AudioType.NormalDialogPlayButton : AudioType.PhoneDialogPlayButton);
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
                dialog.isNeedRecord = episode.isNeedRecord;
                dialog.belongGroup = episode.belongGroup;
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
            nextBtn.gameObject.SetActive(false);
            ChangeStatus(EpisodePlayerStatus.Stop);
            var episode = ConfigController.Instance.GetEpisodeConfig(curEpisodeID);
            if (episode != null)
            {
                if (episode.disableEquipmentID?.Count > 0)
                {
                    foreach (var id in episode.disableEquipmentID)
                    {
                        SceneController.Instance.UpdateEquipment(id, false);
                    }
                }
                if (episode.enableEquipmentID?.Count > 0)
                {
                    foreach (var id in episode.enableEquipmentID)
                    {
                        SceneController.Instance.UpdateEquipment(id, true);
                    }
                }
            }
            GameDataProxy.Instance.finishedEpisode.Add(curEpisodeID);
            GameLineNode node = new GameLineNode();
            node.type = GameNodeType.Episode;
            node.ID = curEpisodeID;
            GameDataProxy.Instance.canOperate = true;
            if (episode.episodeType == EpisodeType.Normal)
            {
                gameObject.SetActive(false);
            }
            else if (episode.episodeType == EpisodeType.Phone)
            {
                UIController.Instance.HidePhoneView();
            }
            if (curEpisodeID == "MS01_060_020")
            {
                AudioController.Instance.PlaySyncAudioEffect(AudioType.TelegraphLong);
            }
            
            curEpisodeID = "";
            MessageManager.Instance.Send(MessageDefine.PlayEpisodeDone, new MessageData(node));
            return;
        }
        var dialog = dialogQueue.Dequeue();
        if (dialog != null)
        {
            UpdateView(dialog);
        }
    }

    protected void ChangeStatus(EpisodePlayerStatus status)
    {
        curStatus = status;
    }

    protected void ShowChoices()
    {
        nextBtn.gameObject.SetActive(false);
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
        //var configController = ConfigController.Instance;
        // 选项
        //if (dialog.choices?.Count > 0)
        //{
        //    foreach (var choiceID in dialog.choices)
        //    {
        //        ChoiceConfig choice = configController.GetChoiceConfig(choiceID);
        //        if (choice != null)
        //        {
        //            choices.Add(choice);
        //        }
        //    }
        //}
        //else
        //{
        //    choices = null;
        //}
        // 获得道具
        if (dialog.getItemID.Count > 0)
        {
            UIController.Instance.GetItemTip(dialog.getItemID);
        }
        if (curType == EpisodeType.Normal) {
            UpdateNormalView(dialog);
        }
        else if (curType == EpisodeType.Phone)
        {
            UpdatePhoneView(dialog);
        }
        if (nextBtn != null)
        {
            nextBtn.gameObject.SetActive(true);
        }
    }

    private void UpdateNormalView(DialogConfig dialog)
    {
        // 打字机
        if (useTypeEffect)
        {
            targetText = dialog.content;
            typeSpeed = normalTypingSpeed;
            typeTimer = 0;
            ChangeStatus(EpisodePlayerStatus.Typing);
            AudioController.Instance.PlayAudioEffect(AudioType.TypeEffect, true);
        }
        else
        {
            content.text = dialog.content;
            ChangeStatus(EpisodePlayerStatus.Pause);
        }
        // 中间的图片
        ImageRes middleImgRes = null;
        if (ResourcesController.Instance.dialogItemRes.ContainsKey(dialog.ID))
        {
            middleImgRes = ResourcesController.Instance.dialogItemRes[dialog.ID];
        }
        if (middleImgRes != null)
        {
            middleImg.gameObject.SetActive(true);
            middleImg.sprite = middleImgRes.sprite;
            middleImg.SetNativeSize();
        }
        else
        {
            middleImg.gameObject.SetActive(false);
        }
        // 立绘、名字
        RoleRes roleRes = null;
        if (ResourcesController.Instance.roleRes.ContainsKey(dialog.roleType))
        {
            roleRes = ResourcesController.Instance.roleRes[dialog.roleType];
        }
        if (roleRes != null)
        {
            roleImg.gameObject.SetActive(true);
            if (roleRes.imageTypeResMap.ContainsKey(dialog.imageType))
            {
                roleImg.gameObject.SetActive(true);
                roleImg.sprite = roleRes.imageTypeResMap[dialog.imageType];
            }
            else
            {
                roleImg.gameObject.SetActive(false);
            }
            nameText.text = roleRes.name;
        }
        else
        {
            nameText.text = "";
            roleImg.gameObject.SetActive(false);
        }
        if (dialog.isNeedRecord)
        {
            GameDataProxy.Instance.normalHistoryDialog.Add(dialog);
        }
    }

    private void UpdatePhoneView(DialogConfig dialog)
    {
        GameObject obj = dialog.roleType == RoleType.MainRoleGirl ? rightDialogObj : leftDialogObj;
        Sprite icon = null;
        string name = "";
        var roleRes = ResourcesController.Instance.roleRes[dialog.roleType];
        if (roleRes != null)
        {
            icon = roleRes.icon;
            name = roleRes.name;
        }
        var view = BaseFunction.CreateView<PhoneEpisodePartView>(obj, contentNode);
        if (view != null)
        {
            ChangeStatus(EpisodePlayerStatus.Pause);
            view.UpdateView(icon, name, dialog.content);
            phoneDialogList.Add(view);
        }
        if (dialog.isNeedRecord)
        {
            if (!GameDataProxy.Instance.phoneHistoryDialog.ContainsKey(dialog.belongGroup))
            {
                GameDataProxy.Instance.phoneHistoryDialog[dialog.belongGroup] = new List<DialogConfig>();
            }
            GameDataProxy.Instance.phoneHistoryDialog[dialog.belongGroup].Add(dialog);
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
