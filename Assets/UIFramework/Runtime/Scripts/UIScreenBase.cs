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
        public ConcurrentDictionary<UIPopupBase, bool> uiPopups { get; private set; }
            = new ConcurrentDictionary<UIPopupBase, bool>();

        public abstract string screenName { get; }

        public object[] parameters;

        public enum State
        {
            created,
            goingShow,
            shown,
            present,
            goingLeave,
            hidden
        }

        public State curState { get; private set; } = State.created;

        /// <summary>
        /// 等待直到达到 target 状态
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public async Task AwaitToTargetState(State targetState)
        {
            while (true)
            {
                if ((int)targetState <= (int)curState)
                {
                    return;
                }
                await this.AwaitNextFrame();
            }
        }

        public async Task UpdateScreenState(State state)
        {
            curState = state;
            switch (state)
            {
                case State.goingShow: await this.OnScreenGoingShow(); break;
                case State.shown:
                    {
                        await this.OnScreenShown();
                        this.curState = State.present;
                        break;
                    }
                case State.goingLeave: await this.OnScreenGoingLeave(); break;
                case State.hidden: await this.OnScreenHidden(); break;
            }
        }

        /// <summary>
        /// component is already instantiated, but not active
        /// </summary>
        protected abstract Task OnScreenGoingShow();

        /// <summary>
        /// component is already instantiated and active, shown in game
        /// </summary>
        protected abstract Task OnScreenShown();

        /// <summary>
        /// component is active, and going to inactive itself
        /// </summary>
        protected abstract Task OnScreenGoingLeave();

        /// <summary>
        /// component is already inactive and going to destroy
        /// </summary>
        protected abstract Task OnScreenHidden();

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

                if (script != null && uiPopups.TryAdd(script, true))
                {
                    script.parameters = parameters;
                    script.screen = this;
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
            await script.UpdatePopupState(UIPopupBase.State.goingShow);
            script.gameObject.SetActive(true);
            await this.AwaitNextFrame();
            await script.UpdatePopupState(UIPopupBase.State.shown);
        }

        public virtual async Task<bool> DestroyPopup(UIPopupBase script)
        {
            if (script == null)
            {
                return false;
            }
          
            // 先从集合中移除，此时已经无法从外部获取到这个Popup
            if (uiPopups.TryRemove(script, out _))
            {
                // 一定等到显示完成在进行销毁
                await script.AwaitAfterTargetState(UIPopupBase.State.present);

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

            await script.UpdatePopupState(UIPopupBase.State.goingLeave);
            script.gameObject?.SetActive(false);

            // Wait for one frame
            await this.AwaitNextFrame();

            await script.UpdatePopupState(UIPopupBase.State.hidden);
            if (script.gameObject != null)
            {
                GameObject.Destroy(script.gameObject);
            }
        }

        public bool ContainsPopup(UIPopupBase popup)
        {
            return uiPopups.ContainsKey(popup);
        }
    }

    public class ScreenContext
    {
        public string key => screen.screenName;

        public UIScreenBase screen;
    }
}
