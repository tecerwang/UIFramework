using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// screen script base
    /// </summary>   
    public abstract class UIScreenBase : MonoBehaviour
    {
        public ConcurrentBag<UIPopupBase> uiPopups { get; private set; } = new ConcurrentBag<UIPopupBase>();

        public abstract string screenName { get; }

        public object[] parameters;

        /// <summary>
        /// component is already instantiated, but not active
        /// </summary>
        public abstract Task OnScreenGoingShow();

        /// <summary>
        /// component is already instantiated and active, shown in game
        /// </summary>
        public abstract Task OnScreenShown();

        /// <summary>
        /// component is active, and going to inactive itself
        /// </summary>
        public abstract Task OnScreenGoingLeave();

        /// <summary>
        /// component is already inactive and going to destroy
        /// </summary>
        public abstract Task OnScreenHidden();

        public virtual async Task<UIPopupBase> CreatePopup(AsyncLoadAsset<GameObject> assetLoader, params object[] parameters)
        {
            try
            {
                // addressable load
                var prefab = await assetLoader.LoadAssetAsync();
                if (prefab == null)
                {
                    Utility.LogDebug(screenName, $"popupPrefab {prefab.name} is missing, please check project assets or Addressable Groups");
                    return null;
                }
                Utility.LogDebug(screenName, $"loaded popupPrefab {prefab.name}");
                // Instantiate
                var instance = GameObject.Instantiate(prefab, gameObject.transform);
                instance.gameObject.SetActive(false);

                // find screen script
                UIPopupBase script = instance.GetComponent<UIPopupBase>();
                instance.name = script.popupName;

                if (script != null)
                {
                    script.parameters = parameters;
                    script.screen = this;

                    // add context to repo
                    uiPopups.Add(script);
                    await HandlePopupAppear(script);
                    Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} add to Scene");
                    return script;
                }
                else
                {
                    GameObject.Destroy(instance);
                    Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} does not contain UIScreenBase Script, will destroy the GameObject which is instantiated");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utility.LogException("UIScreenManager:", ex.ToString());
                return null;
            }
        }

        private async Task HandlePopupAppear(UIPopupBase script)
        {
            if (script == null)
            {
                await Task.CompletedTask;
            }
            Utility.LogDebug("UIScreenManager", $"screenPrefab {script.popupName} HandleScreenDisappear");
            await script.OnPopupGoingShow();
            script.gameObject.SetActive(true);
            await this.AwaitNextFrame();
            await script.OnPopupShown();
        }

        public virtual async Task<bool> DestroyPopup(UIPopupBase script)
        {
            if (script == null)
            {
                return false;
            }

            // Remove the script from the ConcurrentBag
            if (uiPopups.TryTake(out _))
            {
                Utility.LogDebug("UIScreenManager", $"popupPrefab {script.popupName} HandleScreenDisappear");
                await HandlePopupDisappear(script);
                return true;
            }
            else
            {
                Utility.LogDebug("UIScreenManager", $"popupPrefab dose not contains in screen's popup collection");
                return false;
            }
        }

        private async Task HandlePopupDisappear(UIPopupBase script)
        {
            if (script == null)
            {
                await Task.CompletedTask;
            }

            await script.OnPopupGoingLeave();
            script.gameObject?.SetActive(false);

            // Wait for one frame
            await this.AwaitNextFrame();

            await script.OnPopupHidden();
            if (script.gameObject != null)
            {
                GameObject.Destroy(script.gameObject);
            }
        }
    }

    public class ScreenContext
    {
        public string key => screen.screenName;

        public UIScreenBase screen;
    }
}
