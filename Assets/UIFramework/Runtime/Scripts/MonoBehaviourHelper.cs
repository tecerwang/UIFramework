using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    public static class MonoBehaviourHelper
    {
        public static void DestroyChildren(this Transform parent)
        {
            if (parent == null)
            {
                return;
            }
            if (parent.childCount > 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    GameObject.Destroy(parent.GetChild(i).gameObject);
                }
            }
        }
       
        public static async Task AwaitNextFrame(this MonoBehaviour mono)
        {
            // ÄÚ²¿º¯Êý
            IEnumerator DelayOneFrameCoroutine(Action onComplete)
            {
                yield return new WaitForEndOfFrame();
                onComplete?.Invoke();
            }

            var tcs = new TaskCompletionSource<bool>();
            mono.StartCoroutine(DelayOneFrameCoroutine(() => tcs.SetResult(true)));
            await tcs.Task;
        }
    }
}