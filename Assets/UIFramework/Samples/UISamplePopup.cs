using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

public class UISamplePopup : UIPopupBase
{
    public override string popupName => "UISamplePopup";

    public override async UniTask OnPopupGoingLeave()
    {
        Utility.LogDebug(popupName, "OnPopupGoingLeave");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnPopupGoingShow()
    {
        Utility.LogDebug(popupName, "OnPopupGoingShow");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnPopupHidden()
    {
        Utility.LogDebug(popupName, "OnPopupHidden");
        await UniTask.CompletedTask;
    }

    public override async UniTask OnPopupShown()
    {
        Utility.LogDebug(popupName, "OnPopupShown");
        await UniTask.CompletedTask;
    }

    void Awake()
    {
        Utility.LogDebug(popupName, "Awake");
    }

    void Start()
    {
        Utility.LogDebug(popupName, "Start");
    }
}
