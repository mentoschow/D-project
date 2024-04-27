using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    public string equipmentID;

    [SerializeField]
    private GameObject star;  // …¡À∏–«–«
    [SerializeField]
    private float flashTime = 2;

    private EquipmentConfig equipmentConfig;
    private bool interactiveable = true;
    private float timer = 0;

    void Start()
    {
        timer = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    void Update()
    {
        if (timer < flashTime)
        {
            timer += Time.deltaTime;
        }
        else if (timer == flashTime)
        {
            timer = -flashTime;
        }
        
        star.SetActive(timer > 0);
    }
}
