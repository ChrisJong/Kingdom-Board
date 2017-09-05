namespace Selectable {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class SelectableObject : MonoBehaviour, GO {

        protected int _id;
        public int ID {
            set { this._id = value; }
            get { return this._id; }
        }

        public virtual void Destroy() { }

        public virtual void Reset() { }

        public virtual void SetID() {

        }
    }
}