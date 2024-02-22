using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    public class AssetLoaderResource<T> : AsyncLoadAsset<T> where T : Object
    {
        private string _resourcePath;

        public AssetLoaderResource(string resourcePath)
        {
            this._resourcePath = resourcePath;
        }

        public override async Task<T> LoadAssetAsync()
        {
            TaskCompletionSource<T> source = new TaskCompletionSource<T>();
            var request = Resources.LoadAsync<T>(_resourcePath);
            request.completed += (op) => 
            {
                if (request.isDone && request.asset is T)
                {
                    source.SetResult(request.asset as T);
                }
                else
                {
                    source.SetResult(null);
                }
            };
            return await source.Task;
        }

        public override async Task ReleaseAsset()
        {
            await Task.CompletedTask;
        }       
    }
}
