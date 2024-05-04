using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : MonoSingleton<ResourcesController>
{
    [SerializeField]
    private GameRes gameRes;

    public Dictionary<RoleType, RoleRes> roleRes = new Dictionary<RoleType, RoleRes>();
    public Dictionary<BelongPhoneGroup, WechatGroupRes> wechatGroupRes = new Dictionary<BelongPhoneGroup, WechatGroupRes>();
    public Dictionary<string, ImageRes> clueItemRes = new Dictionary<string, ImageRes>();
    public Dictionary<string, ImageRes> dialogItemRes = new Dictionary<string, ImageRes>();
    public Dictionary<StageType, SceneRes> sceneRes = new Dictionary<StageType, SceneRes>();

    void Awake()
    {
        foreach (var role in gameRes.roleRes)
        {
            role.imageTypeResMap = new Dictionary<DialogImageType, Sprite>();
            foreach (var res in role.imageTypeRes)
            {
                role.imageTypeResMap.Add(res.type, res.sprite);
            }
            roleRes[role.type] = role;
        }
        foreach (var group in gameRes.wechatGroupRes)
        {
            wechatGroupRes[group.group] = group;
        }
        foreach (var clue in gameRes.clueItemImage)
        {
            clueItemRes[clue.ID] = clue;
        }
        foreach (var dialog in gameRes.dialogImage)
        {
            dialogItemRes[dialog.ID] = dialog;
        }
        foreach (var scene in gameRes.scenePrefabs)
        {
            sceneRes[scene.type] = scene;
        }
    }
}
