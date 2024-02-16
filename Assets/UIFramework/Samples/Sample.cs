using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using System.Linq;
using System.Threading.Tasks;

namespace UIFrameworkSample
{
    public class Smaple : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (UIScreenManager.singleton.IsInited)
            {
                _ = UIScreenManager.singleton.CreateScreen("UISampleScreen.prefab");
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
            var screen = await UIScreenManager.singleton.CreateScreen("UISampleScreen.prefab");
            if (screen != null)
            {
                await screen.CreatePopup("UISamplePopup.Prefab");
            }
        }
    }
}
