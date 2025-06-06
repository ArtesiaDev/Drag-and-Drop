using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Scripts.Services.AssetManagement
{
    public class AssetProvider : IAssetProvider, IInitializable, IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle> _cache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        public void Initialize() => 
            Addressables.InitializeAsync();

        public void Dispose() => 
            Release();

        public async Task<Sprite> LoadAssetReferenceSprite(AssetReferenceSprite sprite) =>
            await Load<Sprite>(sprite.RuntimeKey.ToString());

        public async Task<T> Load<T>(string key) where T : class
        {
            if (_cache.TryGetValue(key, out var completedHandle))
            {
                return completedHandle.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
             handle.Completed += asyncOperationHandle => 
                 { _cache[key] = asyncOperationHandle; };

            if (!_handles.TryGetValue(key, out var handles))
            {
                handles = new List<AsyncOperationHandle>();
                _handles[key] = handles;
            }

            handles.Add(handle);

            return await handle.Task;
        }

        public void Release(AssetReferenceSprite sprite) 
            => Release(sprite.RuntimeKey.ToString());

        public void Release(string key)
        {
            if (!_handles.TryGetValue(key, out var handle1))
                return;

            foreach (var handle in handle1)
                Addressables.Release(handle);

            _cache.Remove(key);
            _handles.Remove(key);
        }

        private void Release()
        {
            foreach (var handle in _handles.Values.SelectMany(list => list))
                Addressables.Release(handle);

            _cache.Clear();
            _handles.Clear();
        }
    }
}
