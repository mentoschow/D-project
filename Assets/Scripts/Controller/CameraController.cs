using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoSingleton<CameraController>
{
    private Camera gameCam;
    private bool canMove;
    
    void Start()
    {
        gameCam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        var role = RoleController.Instance.curRoleView;
        if (!role)
        {
            return;
        }
        float newX = role.transform.position.x;
        float moveMax = (SceneController.Instance.sceneBgWidth - gameCam.pixelWidth) / 2 / 100;
        if (newX > moveMax)
        {
            canMove = false;
            transform.position = new Vector3(moveMax, 0, -10);
        }
        else if (newX < -moveMax)
        {
            canMove = false;
            transform.position = new Vector3(-moveMax, 0, -10);
        } else
        {
            canMove = true;
        }
        if (canMove)
        {
            transform.position = new Vector3(newX, 0, -10);
        }
    }

    void Update()
    {
        
    }
}
