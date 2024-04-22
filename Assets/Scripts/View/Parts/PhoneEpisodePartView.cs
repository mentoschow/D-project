using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneEpisodePartView : MonoBehaviour
{
    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private Text contentText;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private int maxTextWidth;

    public void UpdateView(Sprite icon, string name, string content)
    {
        iconImg.sprite = icon;
        contentText.text = content;
        nameText.text = name;
        if (contentText.preferredWidth > maxTextWidth)
        {
            contentText.GetComponent<LayoutElement>().preferredWidth = maxTextWidth;
        }
    }

    private void Update()
    {
        var rect = GetComponent<RectTransform>();
        var iconRect = transform.Find("icon")?.GetComponent<RectTransform>();
        var contentRect = transform.Find("content")?.GetComponent<RectTransform>();
        var contentHeight = contentRect.rect.height + 20;
        float newHeight;
        if (contentHeight > iconRect.rect.height)
        {
            newHeight = contentHeight;
        }
        else
        {
            newHeight = iconRect.rect.height;
        }
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }
}
