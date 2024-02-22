using System.Threading.Tasks;
using UIFramework;
using UnityEngine;


namespace UIFrameworkSample.Addressable
{
    public class UISamplePopup : UIPopupBase
    {
        public override string popupName => "UISamplePopup";

        public override async Task OnPopupGoingLeave()
        {
            Utility.LogDebug(popupName, "OnPopupGoingLeave");
            await Task.CompletedTask;
        }

        public override async Task OnPopupGoingShow()
        {
            Utility.LogDebug(popupName, "OnPopupGoingShow");
            await Task.CompletedTask;
        }

        public override async Task OnPopupHidden()
        {
            Utility.LogDebug(popupName, "OnPopupHidden");
            await Task.CompletedTask;
        }

        public override async Task OnPopupShown()
        {
            Utility.LogDebug(popupName, "OnPopupShown");
            await Task.CompletedTask;
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
}
