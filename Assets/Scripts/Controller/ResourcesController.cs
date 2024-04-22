using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : MonoSingleton<ResourcesController>
{
    [SerializeField]
    private GameRes gameRes;

    public Dictionary<RoleType, RoleRes> roleRes = new Dictionary<RoleType, RoleRes>();
    public Dictionary<string, ImageRes> clueItemRes = new Dictionary<string, ImageRes>();
    public Dictionary<string, ImageRes> dialogItemRes = new Dictionary<string, ImageRes>();

    void Start()
    {
        
    }
}
