using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    private TransitionType curType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayTransition(TransitionType type)
    {
        curType = type;
        Invoke("PlayTransitionOver", 2);
    }

    private void PlayTransitionOver()
    {
        gameObject.SetActive(false);
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.Transition;
        node.ID = curType.ToString();
        MessageManager.Instance.Send(MessageDefine.PlayTransitionDone, new MessageData(node));
    }
}
