using System.Collections;
using System.Collections.Generic;
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
    }
}