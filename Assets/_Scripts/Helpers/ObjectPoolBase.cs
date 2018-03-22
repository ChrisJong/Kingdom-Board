namespace Helpers {

    using UnityEngine;

    using Utility;

    public abstract class ObjectPoolBase : MonoBehaviour, IObjectPool {
        #region VARIBALE
        [SerializeField, ReadOnly]
        protected uint _poolID;
        public uint id {
            get { return this._poolID; }
        }
        #endregion

        #region UNITY
        protected virtual void OnEnable() {
            this._poolID = Utils.nextPoolID;
        }
        #endregion

        #region SYSTEM
        public override bool Equals(object other) {
            IObjectPool o = other as IObjectPool;

            if(o == null)
                return false;

            return o.id == this._poolID;
        }

        public override int GetHashCode() {
            return this._poolID.GetHashCode();
        }

        public static bool operator ==(ObjectPoolBase a, ObjectPoolBase b) {
            object oA = (object)a;
            object oB = (object)b;

            if(oA == null && oB == null)
                return true;

            if(oA == null || oB == null)
                return false;

            return a.id == b.id; 
        }

        public static bool operator !=(ObjectPoolBase a, ObjectPoolBase b) {
            return !a == b;
        }

        #endregion
    }
}