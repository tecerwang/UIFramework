using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace UIFrameworkSample.Addressable
{
    public class Smaple : MonoBehaviour
    {
        public Button btnAddScreen;
        public Button btnRemoveScreen;

        // Start is called before the first frame update
        async void Start()
        {
            await UIScreenManager.AwaitForInitComplete();

            var screen = new AddressableAssetLoader<GameObject>("Screens/UISampleScreen.prefab");
            _ = UIScreenManager.singleton.CreateScreen(screen);

            btnAddScreen.onClick.AddListener(() => 
            {
                _ = CreateUIScreenAndPopup(); 
            });

            btnRemoveScreen.onClick.AddListener(() =>
            {
                KeyValuePair<int, UIScreenBase>? pair = UIScreenManager.singleton.uiScreens.FirstOrDefault();
                _ = UIScreenManager.singleton.DestroyScreen(pair?.Value);
            });
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
