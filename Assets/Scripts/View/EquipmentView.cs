using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    public int equipmentID;

    private EquipmentConfig equipmentConfig;
    private PolygonCollider2D pCollider;
    private BoxCollider2D trigger;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //equipmentConfig = ConfigController.Instance.GetEquipment(equipmentID);
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            spriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        spriteRenderer.color = Color.white;
    }

    void Update()
    {
        
    }
}
