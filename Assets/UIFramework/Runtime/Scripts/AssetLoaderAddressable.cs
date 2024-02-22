using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if ADDRESSABLES_ENABLED
namespace UIFramework
{
    public class AddressableAssetLoader<T> : AsyncLoadAsset<T> where T : Object
    {
        private string _addressablePath;

        public AddressableAssetLoader(string addressblePath)
        {
            _addressablePath = addressblePath;
        }

        public override async Task<T> LoadAssetAsync()
        {
            if (string.IsNullOrEmpty(_addressablePath))
            {               
                return default(T);
            }

            return await Addressables.LoadAssetAsync<T>(_addressablePath).Task;
        }

        public override async Task ReleaseAsset()
        {
            await Task.CompletedTask;
        }
    }
}
#endif