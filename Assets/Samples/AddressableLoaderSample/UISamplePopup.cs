using System.Threading.Tasks;
using UIFramework;
using UnityEngine;


namespace UIFrameworkSample.Addressable
{    
    public class UISamplePopup : UIPopupBase
    {
        public override string popupName => "UISamplePopup";

        public GameObject Text1;
        public GameObject Text2;
        public GameObject Text3;
        public GameObject Text4;
        protected override async Task OnPopupGoingShow()
        {
            Utility.LogDebug(popupName, "OnPopupGoingShow");
            Text1.SetActive(true);
            // ≤‚ ‘“Ï≤Ω ±–Ú
            await Task.Delay(2000);
            await Task.CompletedTask;
        }

        protected override async Task OnPopupShown()
        {
            Utility.LogDebug(popupName, "OnPopupShown");
            Text2.SetActive(true);
            await Task.CompletedTask;
        }

        protected override async Task OnPopupGoingLeave()
        {
            Utility.LogDebug(popupName, "OnPopupGoingLeave");
            Text3.SetActive(true);
            Text4.SetActive(true);
            await Task.Delay(2000);
            await Task.CompletedTask;
        }


        protected override async Task OnPopupHidden()
        {
            Utility.LogDebug(popupName, "OnPopupHidden");

            await Task.Delay(2000);
            // ≤‚ ‘“Ï≤Ω ±–Ú
            await Task.CompletedTask;
        }

        void Awake()
        {
            Utility.LogDebug(popupName, "Awake");
            Text1.SetActive(false);
            Text2.SetActive(false);
            Text3.SetActive(false);
            Text4.SetActive(false);
        }

        void Start()
        {
            Utility.LogDebug(popupName, "Start");
        }
    }
}
