﻿namespace Helpers {

    using UnityEngine;

    using Enum;
    using Manager;

    [RequireComponent(typeof(Collider))]
    public abstract class EntityBase : ObjectPoolBase, IEntity {

        #region VARIABLE
        public abstract EntityType entityType { get; }

        public Vector3 position {
            get { return this.transform.position; }
            set { this.transform.position = value; } }
        public Quaternion rotation {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; } }
        public bool isActive {
            get { return this.gameObject.activeSelf; }
            set { this.gameObject.SetActive(value); } }
        #endregion

        #region UNITY
        protected override void OnEnable() {
            base.OnEnable();

            this.Link(this.GetComponent<Collider>());
        }

        protected virtual void OnDisable() {
            this.Unlink(this.GetComponent<Collider>());
        }
        #endregion
    }
}