using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonButtonPartView : MonoBehaviour
{
    [SerializeField]
    private Button quitBtn;
    [SerializeField]
    private Button clueBtn;

    void Start()
    {
        quitBtn?.onClick.AddListener(BackToHomepage);
        clueBtn?.onClick.AddListener(ShowClueOrPhone);
    }

    private void BackToHomepage()
    {
        //UIController.Instance.BackHomepage();
        AudioController.Instance.PlayAudioEffect(AudioEffectType.NormalButton);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowClueOrPhone()
    {
        AudioController.Instance.PlayAudioEffect(AudioEffectType.NormalButton);
        UIController.Instance.ShowPhoneOrClue();
    }
}
