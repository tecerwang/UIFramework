using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using System.Linq;
using System.Threading.Tasks;

namespace UIFrameworkSample.Addressable
{
    public class Smaple : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (UIScreenManager.singleton.IsInited)
            {
               var screen = new AddressableAssetLoader<GameObject>("Screens/UISampleScreen.prefab");
                _ = UIScreenManager.singleton.CreateScreen(screen);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                KeyValuePair<string, ScreenContext>? pair = UIScreenManager.singleton.uiScreens.FirstOrDefault();
                _ = UIScreenManager.singleton.DestroyScreen(pair?.Value);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _ = CreateUIScreenAndPopup();
            }
        }

        private async Task CreateUIScreenAndPopup()
        {
            var screenloader = new AddressableAssetLoader<GameObject>("Screens/UISampleScreen.prefab");           
            if (screenloader != null)
            {
                var screenScript = await UIScreenManager.singleton.CreateScreen(screenloader);
                var popuploader = new AddressableAssetLoader<GameObject>("Popups/UISamplePopup.Prefab");
                await screenScript.CreatePopup(popuploader);
            }
        }
    }
}
