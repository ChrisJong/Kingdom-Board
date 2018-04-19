namespace Helpers {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public abstract class PoolBase<T> where T : IObjectPool {

        private Queue<T> _pool;
        private GameObject _prefab;
        private GameObject _host;

        public PoolBase(GameObject prefab, GameObject host, int initialInstanceCount) {
            this._pool = new Queue<T>(initialInstanceCount);
            this._prefab = prefab;
            this._host = host;

            for(int i = 0; i < initialInstanceCount; i++)
                this._pool.Enqueue(this.CreateInstance());
        }

        public int Count {
            get { return this._pool.Count; }
        }

        public T Get(Vector3 position, Quaternion rotation) {
            T entity;

            Debug.Log(this._pool.Count);

            if(this._pool.Count > 0)
                entity = this._pool.Dequeue();
            else
                entity = this.CreateInstance();

            var t = entity.transform;
            t.position = position;
            t.rotation = rotation;

            entity.gameObject.SetActive(true);
            return entity;
        }

        public void Return(T entity) {
            entity.transform.SetParent(this._host.transform);
            entity.gameObject.SetActive(false);
            this._pool.Enqueue(entity);
        }

        private T CreateInstance() {
            var go = GameObject.Instantiate(this._prefab);
            go.transform.SetParent(this._host.transform);
            go.SetActive(false);
            return go.GetComponent<T>();
        }
    }
}