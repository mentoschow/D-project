using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleItemView : MonoSingleton<PuzzleItemView>
{
    public Image pic;
    // Start is called before the first frame update
    // Update is called once per frame
    private void Awake()
    {
        //Debug.Log("awake");
        pic = transform.Find("pic")?.GetComponent<Image>();
    }

    public void updateView(PuzzleItemConfig config)
    {
        //Debug.Log("updateView");
        CommonUtils.updateImage(config?.url, pic);
    }
}
