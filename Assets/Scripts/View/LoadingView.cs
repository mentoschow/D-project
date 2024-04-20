using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayTransition(TransitionType type)
    {
        Invoke("PlayTransitionOver", 2);
    }

    private void PlayTransitionOver()
    {
        gameObject.SetActive(false);
    }
}
