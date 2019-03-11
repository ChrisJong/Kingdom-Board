namespace KingdomBoard.Helpers {

    using System.Collections.Generic;

    using UnityEngine;

    public class RaycastHitDistanceSortComparer : IComparer<RaycastHit> {

        #region VARIBALE
        private Vector3 _position;
        private int _sortDir;

        public bool ascending {
            get { return this._sortDir == 1; }
            set { this._sortDir = value ? 1 : -1; } }

        public Vector3 position {
            get { return this._position; }
            set { this._position = value; } }
        #endregion

        #region CLASS
        public RaycastHitDistanceSortComparer(bool ascending) : this(Vector3.zero, ascending) { }
        public RaycastHitDistanceSortComparer(Vector3 position , bool ascneding) {
            this._position = position;
            this.ascending = ascending;
        }

        public int Compare(RaycastHit a, RaycastHit b) {
            if(a.transform == null)
                return 1 * this._sortDir;

            if(b.transform == null)
                return -1 * this._sortDir;

            return (a.point - this._position).sqrMagnitude.CompareTo((b.point - this._position).sqrMagnitude) * this._sortDir;
        }
        #endregion
    }
}