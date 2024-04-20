using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItemTipPartView : MonoBehaviour
{
    public Sprite bg_boy_normal;
    public Sprite bg_boy_click;
    public Sprite bg_girl_normal;
    public Sprite bg_girl_click;

    private Text text;
    private Button button;

    void Awake()
    {
        button = transform.Find("Button")?.GetComponent<Button>();
        text = button?.transform.Find("Text")?.GetComponent<Text>();
        button.onClick.AddListener(OnClick);
    }

    public void UpdateView(string itemID)
    {
        var config = ConfigController.Instance.GetItemConfig(itemID);
        if (config != null)
        {
            var curRoleType = RoleController.Instance.curRoleView.characterType;
            SpriteState spriteState = new SpriteState();
            if (curRoleType == RoleType.MainRoleBoy)
            {
                spriteState.highlightedSprite = bg_boy_normal;
                spriteState.pressedSprite = bg_boy_click;
            }
            else if (curRoleType == RoleType.MainRoleGirl)
            {
                spriteState.highlightedSprite= bg_girl_normal;
                spriteState.pressedSprite= bg_girl_click;
            }
            button.spriteState = spriteState;
            text.text = "获得道具：" + config.name;
        }
        Invoke("Done", 2);
    }

    private void OnClick()
    {

    }

    private void Done()
    {
        Destroy(gameObject);
    }
}
