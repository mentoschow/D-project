using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneWechatPartView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text dialogPreviewText;
    [SerializeField]
    private Button button;
    [SerializeField]
    private int maxTextWidth;

    private BelongPhoneGroup group;

    void Start()
    {
        button?.onClick.AddListener(OnButtonClick);
    }

    public void UpdateView(BelongPhoneGroup _group, DialogConfig dialog)
    {
        group = _group;
        if (ResourcesController.Instance.roleRes.ContainsKey(dialog.roleType))
        {
            var res = ResourcesController.Instance.roleRes[dialog.roleType];
            icon.sprite = res.icon;
            nameText.text = res.name;
        }
        dialogPreviewText.text = dialog.content;
        var newString = BaseFunction.StripLength(dialogPreviewText, maxTextWidth);
        if (dialogPreviewText.text != newString)
        {
            dialogPreviewText.text = newString + "...";
        }
    }

    private void OnButtonClick()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.PhoneButton);
        MessageManager.Instance.Send(MessageDefine.OpenWechatDialogPage, new MessageData(group));
    }
}
