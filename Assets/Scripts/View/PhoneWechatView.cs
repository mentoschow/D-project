using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhoneWechatView : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private GameObject partObj;

    public void UpdateView()
    {
        var dialogData = GameDataProxy.Instance.phoneHistoryDialog;
        if (dialogData.Count > 0)
        {
            foreach (var dialog in dialogData)
            {
                var view = BaseFunction.CreateView<PhoneWechatPartView>(partObj, content);
                if (view != null)
                {
                    view.UpdateView(dialog.Key, dialog.Value.Last());
                }
            }
        }
    }
}
