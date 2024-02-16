using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIFramework;
using UnityEngine;

public class UISampleScreen : UIScreenBase
{
    public override string screenName => "UISampleScreen";

    public override async UniTask OnScreenGoingLeave()
    {
        Utility.LogDebug(screenName, "OnScreenGoingLeave");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnScreenGoingShow()
    {
        Utility.LogDebug(screenName, "OnScreenGoingShow");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnScreenHidden()
    {
        Utility.LogDebug(screenName, "OnScreenHidden");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnScreenShown()
    {
        Utility.LogDebug(screenName, "OnScreenShown");
        await UniTask.CompletedTask;
    }

    private void Awake()
    {
        Utility.LogDebug(screenName, "Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Utility.LogDebug(screenName, "Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
