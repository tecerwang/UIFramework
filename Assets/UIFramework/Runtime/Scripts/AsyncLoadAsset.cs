using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// �첽���� Asset �ӿ�
    /// </summary>
    public abstract class AsyncLoadAsset<T> where T : Object
    {
        public abstract Task<T> LoadAssetAsync();

        public abstract Task ReleaseAsset();
    }
}
