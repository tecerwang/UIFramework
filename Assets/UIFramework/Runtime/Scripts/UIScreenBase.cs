using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{   
    /// <summary>
    /// screen script base
    /// </summary>   
    public abstract class UIScreenBase : MonoBehaviour
    {
        public List<UIPopupBase> uiPopups { get; private set; } = new List<UIPopupBase>();

        public abstract string screenName { get; }

        public object[] paramaters;

        /// <summary>
        /// component is already instantiated,but not actived
        /// </summary>
        public abstract Task OnScreenGoingShow();

        /// <summary>
        /// component is already instantiated and actived, shown in game
        /// </summary>
        public abstract Task OnScreenShown();

        /// <summary>
        /// component is actived, and going to inactive self
        /// </summary>
        public abstract Task OnScreenGoingLeave();

        /// <summary>
        /// component is already inactived and going to destory
        /// </summary>
        public abstract Task OnScreenHidden();

        public virtual async Task<UIPopupBase> CreatePopup(AsyncLoadAsset<GameObject> assetLoader, params object[] paramaters)
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
                // 实例化
                var instance = GameObject.Instantiate(prefab, gameObject.transform);
                instance.gameObject.SetActive(false);

                // find screen script
                UIPopupBase script = instance.GetComponent<UIPopupBase>();
                instance.name = script.popupName;

                if (script != null)
                {
                    script.paramaters = paramaters;
                    // add context to repo
                    uiPopups.Add(script);
                    _ = HandlePopupAppear(script);
                    Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} add to Scene");
                    return script;
                }
                else
                {
                    GameObject.Destroy(instance);
                    Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} does not contains UIScreenBase Script, will destory the gameobject whitch is instantiated");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utility.LogExpection("UIScreenManager:", ex.ToString());
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

        public virtual async Task DestroyPopup(UIPopupBase script)
        {
            if (script == null)
            {
                await Task.CompletedTask;
                return;
            }
            if (uiPopups.Contains(script))
            {
                uiPopups.Remove(script);
            }
            Utility.LogDebug("UIScreenManager", $"popupPrefab {script.popupName} HandleScreenDisappear");
            await HandlePopupDisappear(script);
        }

        private async Task HandlePopupDisappear(UIPopupBase script)
        {
            if (script == null)
            {
                await Task.CompletedTask;
            }

            await script.OnPopupGoingLeave();
            script.gameObject?.SetActive(false);
            
            // 等待一帧
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
