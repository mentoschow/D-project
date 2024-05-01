using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    [SerializeField]
    private float time = 1.5f;

    private TransitionType curType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayTransition()
    {
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
