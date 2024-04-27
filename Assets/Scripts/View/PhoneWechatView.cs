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

    private List<PhoneWechatPartView> partViews = new List<PhoneWechatPartView>();

    public void UpdateView()
    {
        var dialogData = GameDataProxy.Instance.phoneHistoryDialog;
        if (dialogData.Count > 0)
        {
            for (int i = 0; i < dialogData.Count; i++)
            {
                var keys = new List<BelongPhoneGroup>(dialogData.Keys);
                var values = new List<List<DialogConfig>>(dialogData.Values);
                var view = BaseFunction.CreateView<PhoneWechatPartView>(partObj);
                if (partViews.Count <= i)
                {
                    partViews.Add(view);
                    view.transform.SetParent(content);
                }
                else
                {
                    view = partViews[i];
                }
                if (view != null)
                {
                    view.UpdateView(keys[i], values[i].Last());
                }
            }
        }
    }
}
