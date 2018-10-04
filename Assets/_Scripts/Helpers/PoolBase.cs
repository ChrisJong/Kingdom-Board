namespace Helpers {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// Generic base class for all instance pools. Reduces memory allocatiosns drastically by pooling instances, while possibly increaing the memory footprint.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolBase<T> where T : IObjectPool {

        private Queue<T> _pool;
        private GameObject _prefab;
        private GameObject _host;

        /// <summary>
        /// initializes a new instnace of the <see cref="EntityBase"/> class. 
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="host"></param>
        /// <param name="initialInstanceCount"></param>
        public PoolBase(GameObject prefab, GameObject host, int initialInstanceCount) {
            this._pool = new Queue<T>(initialInstanceCount);
            this._prefab = prefab;
            this._host = host;

            for(int i = 0; i < initialInstanceCount; i++)
                this._pool.Enqueue(this.CreateInstance());
        }

        /// <summary>
        /// Gets the count, the current size of the queue.
        /// </summary>
        public int Count {
            get { return this._pool.Count; }
        }

        /// <summary>
        /// Sets an entity from the pool and places it at the specified position and rotation. If the pool is empty a new instance is created.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The entity</returns>
        public T Get(Vector3 position, Quaternion rotation) {
            T entity;

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

        /// <summary>
        /// returns the specified entity to the pool.
        /// </summary>
        /// <param name="entity"></param>
        public void Return(T entity) {
            entity.transform.SetParent(this._host.transform);
            entity.gameObject.SetActive(false);

            this._pool.Enqueue(entity);
        }

        /// <summary>
        /// Creates a new instance of the entity and adds it into the pool.
        /// </summary>
        /// <returns></returns>
        private T CreateInstance() {
            var go = GameObject.Instantiate(this._prefab);
            go.transform.SetParent(this._host.transform);
            go.SetActive(false);
            return go.GetComponent<T>();
        }
    }
}