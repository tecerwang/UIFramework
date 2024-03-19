using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// Screen ������
    /// </summary>
    public class UIScreenManager : MonoBehaviour
    {
        public ConcurrentDictionary<int, UIScreenBase> uiScreens { get; private set; } = new ConcurrentDictionary<int, UIScreenBase>();

        public static UIScreenManager singleton;

        private TaskCompletionSource<bool> initCompleteTCS = new TaskCompletionSource<bool>();

        /// <summary>
        /// �Ƿ���ɳ�ʼ��
        /// </summary>
        public bool IsInited { get; private set; } = false;

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                singleton.gameObject.name = "[UIScreenManager]";
                DontDestroyOnLoad(singleton.gameObject);
                IsInited = true;
                initCompleteTCS.SetResult(true);
            }
        }

        public static async Task AwaitForInitComplete()
        {
            while (singleton == null)
            {
                await MonoBehaviourHelper.AwaitNextFrame();
            }
            if (singleton.initCompleteTCS.Task.IsCompleted)
            {
                return;
            }
            await singleton.initCompleteTCS.Task;
        }

        /// <summary>
        /// ��ʾһ�� screen
        /// </summary>
        /// <typeparam name="T">��Ļ script ����</typeparam>
        /// <param name="path"> addressable path ,</param>
        /// <param name="parameters">����</param>
        /// <returns>������Ļ�ű�ʵ��</returns>
        public async Task<UIScreenBase> CreateScreen(AsyncLoadAsset<GameObject> assetLoader, params object[] parameters)
        {
            if (assetLoader != null)
            {
                try
                {
                    // addressable load
                    var prefab = await assetLoader.LoadAssetAsync();
                    if (prefab == null)
                    {
                        Utility.LogDebug("UIScreenManager", $"screenPrefab {prefab.name} is missing, please check project assets or Addressable Groups");
                        return null;
                    }
                    Utility.LogDebug("UIScreenManager", $"loaded screenPrefab {prefab.name}");
                    // ʵ����
                    var instance = GameObject.Instantiate(prefab);
                    instance.gameObject.SetActive(false);

                    // find screen script
                    UIScreenBase script = instance.GetComponent<UIScreenBase>();
                    if (script != null)
                    {
                        instance.name = script.screenName;

                        if (uiScreens.TryAdd(script.GetHashCode(), script))
                        {
                            script.parameters = parameters;
                            await HandleScreenAppear(script);
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
                        Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} does not contain UIScreenBase Script, will destroy the gameobject which is instantiated");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Utility.LogException("UIScreenManager:", ex.ToString());
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// ���һ�����е� Screen
        /// </summary>
        /// <param name="screenInstance"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public async Task<UIScreenBase> PushExistScreen(GameObject instance, params object[] parameters)
        {
            // find screen script
            UIScreenBase script = instance.GetComponent<UIScreenBase>();
            if (script != null)
            {
                instance.name = script.screenName;
                if (uiScreens.TryAdd(script.GetHashCode(), script))
                {
                    script.parameters = parameters;
                    await HandleScreenAppear(script);
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
                Utility.LogDebug("UIScreenManager", $"screenPrefab {instance.name} does not contain UIScreenBase Script, will destroy the gameobject which is instantiated");
                return null;
            }
        }

        private async Task HandleScreenAppear(UIScreenBase screen)
        {
            if (screen == null)
            {
                await Task.CompletedTask;
            }
            Utility.LogDebug("UIScreenManager", $"screenPrefab {screen.name} HandleScreenAppear");
            await screen.UpdateScreenState(UIScreenBase.State.goingShow);
            screen.gameObject.SetActive(true);
            await MonoBehaviourHelper.AwaitNextFrame();
            await screen.UpdateScreenState(UIScreenBase.State.shown);
        }

        public async Task<bool> DestroyScreen(UIScreenBase screen)
        {
            if (screen == null)
            {
                return false;
            }
            if (uiScreens.TryRemove(screen.GetHashCode(), out _))
            {
                Utility.LogDebug("UIScreenManager", $"screenPrefab {screen.name} HandleScreenDisappear");
                await HandleScreenDisappear(screen);
                return true;
            }
            else
            {
                Utility.LogDebug("UIScreenManager", $"does not contains screenPrefab {screen.name}");
                return false;
            }
        }

        private async Task HandleScreenDisappear(UIScreenBase screen)
        {
            if (screen == null)
            {
                await Task.CompletedTask;
            }

            // һ���ȵ���ʾ����ڽ�������
            await screen.AwaitToTargetState(UIScreenBase.State.shown);

            // ��Ҫͬʱж��Popup
            foreach (var popup in screen.uiPopups.AsParallel())
            {
                await screen.DestroyPopup(popup.Key);
            }

            await screen.UpdateScreenState(UIScreenBase.State.goingLeave);
            screen.gameObject?.SetActive(false);
            await screen.UpdateScreenState(UIScreenBase.State.hidden);
            if (screen.gameObject != null)
            {
                GameObject.Destroy(screen.gameObject);
            }
        }
    }
}
