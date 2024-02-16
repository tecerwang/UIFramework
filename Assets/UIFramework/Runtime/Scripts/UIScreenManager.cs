using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UIFramework
{
    /// <summary>
    /// Screen 管理类
    /// </summary>
    public class UIScreenManager : MonoBehaviour
    {
        public Dictionary<string, ScreenContext> uiScreens { get; private set; } = new Dictionary<string, ScreenContext>();

        public static UIScreenManager singleton;

        /// <summary>
        /// 是否完成初始化
        /// </summary>
        public bool IsInited { get; private set; } = false;

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                DontDestroyOnLoad(singleton.gameObject);
                IsInited = true;
                OnUIScreenManagerInited?.Invoke();
            }
        }

        /// <summary>
        /// UIScreenManager 初始化完成
        /// </summary>
        public event Action OnUIScreenManagerInited;

        /// <summary>
        /// 显示一个 screen
        /// </summary>
        /// <typeparam name="T">屏幕 script 类型</typeparam>
        /// <param name="path"> addressable path ,</param>
        /// <param name="paramaters">参数</param>
        /// <returns>返回屏幕脚本实例</returns>
        public async UniTask<UIScreenBase> CreateScreen(string addressablePath, params object[] paramaters)
        {
            if (!string.IsNullOrEmpty(addressablePath))
            {
                try
                {
                    // addressable load
                    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Screens/{addressablePath}");
                    var prefab = await handle.ToUniTask<GameObject>();
                    if (prefab == null)
                    {
                        Utility.LogDebug("UIScreenManager", $"screenPrefab {prefab.name} is missing, please check project assets or Addressable Groups");
                        return null;
                    }
                    Utility.LogDebug("UIScreenManager", $"loaded screenPrefab {prefab.name}");
                    // 实例化
                    var instance = GameObject.Instantiate(prefab);
                    instance.gameObject.SetActive(false);
                   
                    // find screen script
                    UIScreenBase script = instance.GetComponent<UIScreenBase>();
                    instance.name = script.screenName;

                    if (script != null)
                    {
                        // create screen context
                        var context = new ScreenContext()
                        {
                            screen = script
                        };
                        if (!uiScreens.ContainsKey(context.key))
                        {
                            script.paramaters = paramaters;
                            // add context to repo
                            uiScreens.Add(context.key, context);
                            _ = HandleScreenAppear(context);
                            Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} add to Scene");
                            return script;
                        }
                        else
                        {
                            GameObject.Destroy(instance);
                            Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} already created, a screen prefab can be created once only");
                            return null;
                        }
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
            return null;
        }

        private async UniTask HandleScreenAppear(ScreenContext context)
        {
            if (context == null)
            {
                await UniTask.CompletedTask;
            }
            Utility.LogDebug("UIScreenManager", $"screenPrefab {context.screen.name} HandleScreenAppear");
            await context.screen.OnScreenGoingShow();
            context.screen.gameObject.SetActive(true);
            await UniTask.NextFrame();
            await context.screen.OnScreenShown();

        }

        public async UniTask DestroyScreen(ScreenContext context)
        {
            if (context == null)
            {
                await UniTask.CompletedTask;
                return;
            }
            if (uiScreens.ContainsKey(context.key))
            {
                uiScreens.Remove(context.key);
            }
            Utility.LogDebug("UIScreenManager", $"screenPrefab {context.screen.name} HandleScreenDisappear");
            await HandleScreenDisappear(context);
        }

        private async UniTask HandleScreenDisappear(ScreenContext context)
        {
            if (context == null)
            {
                await UniTask.CompletedTask;
            }

            // 需要同时卸载Popup
            foreach (var popup in context.screen.uiPopups)
            {
                await popup.OnPopupGoingLeave();
            }
            await context.screen.OnScreenGoingLeave();
            context.screen.gameObject?.SetActive(false);

            // 等待一帧
            await UniTask.NextFrame();

            // 需要同时卸载Popup
            foreach (var popup in context.screen.uiPopups)
            {
                await popup.OnPopupHidden();
            }
            await context.screen.OnScreenHidden();
            if (context.screen.gameObject != null)
            {
                GameObject.Destroy(context.screen.gameObject);
            }
        }       
    }
}
