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
        var res = ResourcesController.Instance.roleRes[dialog.roleType];
        if (res != null)
        {
            icon.sprite = res.icon;
            nameText.text = res.name;
            dialogPreviewText.text = dialog.content;
        }
        var newString = BaseFunction.StripLength(dialogPreviewText, maxTextWidth);
        dialogPreviewText.text = newString + "...";
    }

    private void OnButtonClick()
    {
        MessageManager.Instance.Send(MessageDefine.OpenWechatDialogPage, new MessageData(group));
    }
}
