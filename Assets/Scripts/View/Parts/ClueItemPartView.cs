using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueItemPartView : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text content;
    [SerializeField]
    private Image icon;

    private ItemConfig item;

    void Awake()
    {
        button?.onClick.AddListener(OnButtonClick);
    }

    public void UpdateView(ItemConfig _item)
    {
        if (_item != null) 
        {
            item = _item;
            content.text = item.name;
            if (ResourcesController.Instance.clueItemRes.ContainsKey(item.ID))
            {
                icon.sprite = ResourcesController.Instance.clueItemRes[item.ID].sprite;
            }
        }
    }

    private void OnButtonClick()
    {
        AudioController.Instance.PlayAudioEffect(AudioType.PhoneButton);
        MessageManager.Instance.Send(MessageDefine.ClueItemClick, new MessageData(item.ID));
    }
}
