using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    public string equipmentID;
    public DoorType doorType;
    [SerializeField]
    private SpriteRenderer star;  // ΙΑΛΈΠΗΠΗ
    [SerializeField]
    private float flashTime = 1f;
    public bool interactive = true;
    public bool defaultVisiable = true;

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

    void FixedUpdate()
    {
        star.gameObject.SetActive(interactive);
        if (timer < flashTime)
        {
            timer += Time.deltaTime;
        }
        else 
        {
            timer = -flashTime;
        }
        var curColor = star.color;
        curColor.a = (Mathf.Abs(timer) + 0.3f) / (flashTime + 0.3f);
        star.color = curColor;
    }
}
