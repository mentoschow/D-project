using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoSingleton<CameraController>
{
    public Camera gameCam;

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
        float moveMax = (SceneController.Instance.sceneBgWidth - gameCam.pixelWidth) / 2 / 9.6f;
        if (newX > moveMax)
        {
            canMove = false;
        }
        else if (newX < -moveMax)
        {
            canMove = false;
        } else
        {
            canMove = true;
        }
        if (canMove)
        {
            transform.position = new Vector3(newX, 0, -1);
        }
    }

    void Update()
    {
        
    }
}
