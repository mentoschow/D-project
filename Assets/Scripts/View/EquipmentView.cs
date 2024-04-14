using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    public int equipmentID;

    private EquipmentConfig equipmentConfig;
    private PolygonCollider2D pCollider;
    private BoxCollider2D trigger;

    void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
        equipmentConfig = ConfigController.Instance.GetEquipment(equipmentID);
        if (equipmentConfig == null && equipmentConfig.isCollider)
        {

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
        }
    }

    void Update()
    {
        
    }
}
