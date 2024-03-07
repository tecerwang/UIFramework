using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIFramework;
using UnityEngine;


namespace UIFrameworkSample.Addressable
{
    public class UISampleScreen : UIScreenBase
    {
        public override string screenName => "UISampleScreen";

        protected override async Task OnScreenGoingLeave()
        {
            Utility.LogDebug(screenName, "OnScreenGoingLeave");
            await Task.CompletedTask;
        }

        protected override async Task OnScreenGoingShow()
        {
            Utility.LogDebug(screenName, "OnScreenGoingShow");
            await Task.CompletedTask;
        }

        protected override async Task OnScreenHidden()
        {
            Utility.LogDebug(screenName, "OnScreenHidden");
            await Task.CompletedTask;
        }

        protected override async Task OnScreenShown()
        {
            Utility.LogDebug(screenName, "OnScreenShown");
            await Task.CompletedTask;
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
}
