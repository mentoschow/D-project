using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager
{
    public static ControllerManager GetManager()
    {
        if (_manager == null)
        {
            _manager = new ControllerManager();
        }
        return _manager;
    }
    public static ControllerManager _manager = null;  // µ¥Àý

    public StoryController storyController = new StoryController();
    public UIController UIController = new UIController();
    public GameController gameController = new GameController();
    
}
