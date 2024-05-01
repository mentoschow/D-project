using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    [SerializeField]
    private Text content;
    [SerializeField]
    private float time = 1.5f;

    private TransitionType curType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayTransition(string str)
    {
        content.text = str;
        Invoke("PlayTransitionOver", time);
    }

    public void PlayTransition(TransitionType type)
    {

    }

    private void PlayTransitionOver()
    {
        gameObject.SetActive(false);
    }
}
