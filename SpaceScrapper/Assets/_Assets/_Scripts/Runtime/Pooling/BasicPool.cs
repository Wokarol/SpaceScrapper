using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Wokarol.SpaceScrapper.Pooling
{
    public class BasicPool<T> : MonoBehaviour where T : Component, IPoolable<T>
    {
        Dictionary<T, ObjectPool<T>> poolsByPrefabs = new();

        Transform holder = null;

        public T Get(T prefab, Vector3 position, Quaternion rotation)
        {
            if (!poolsByPrefabs.TryGetValue(prefab, out var pool))
            {
                var newPool = new ObjectPool<T>(
                createFunc: () =>
                {
                    var holder = CreateOrGetHolder();

                    var inst = Instantiate(prefab, holder);
                    inst.OriginalPrefab = prefab;
                    inst.Pool = this;
                    return inst;
                },
                actionOnGet: e => e.gameObject.SetActive(true),
                actionOnRelease: e => e.gameObject.SetActive(false));

                pool = newPool;
                poolsByPrefabs[prefab] = pool;
            }

            var instance = pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }

        private Transform CreateOrGetHolder()
        {
            if (holder == null)
            {
                var root = transform.root;
                string suffix = "";
                if (root != transform)
                {
                    suffix = $" - {root.name}";
                }

                holder = new GameObject($"Pool Holder ({name}{suffix})").transform;
            }

            return holder;
        }

        internal void Return(T obj)
        {
            if (!poolsByPrefabs.TryGetValue(obj.OriginalPrefab, out var pool))
            {
                Debug.LogError("The object does not have a pool created, that should not happen.", this);
                Destroy(obj.gameObject);
            }

            pool.Release(obj);
        }
    }

    public interface IPoolable<T> where T : Component, IPoolable<T>
    {
        public T OriginalPrefab { get; set; }
        public BasicPool<T> Pool { get; set; }
    }
}
