using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFunction
{
    public static void UpdateActive(Component component, bool active)
    {
        if (component)
        {
            component.gameObject.SetActive(active);
        }
    }

    public static void UpdateActive(GameObject gameObject, bool active)
    {
        if (gameObject)
        {
            gameObject.SetActive(active);
        }
    }
}
