namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class PlayerSelection : MonoBehaviour {
        #region VARIABLES

        private Ray _ray;
        private RaycastHit _hitInfo;
        public Vector3 _point;
        public float _distance = 50.0f;

        #endregion

        #region UNITY_METHODS
        private void FixedUpdate() {
            this.CastRayToWorld();
        }
        #endregion

        #region METHODS
        private void CastRayToWorld() {
            this._ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            this._point = this._ray.origin + (this._ray.direction * _distance);

            Debug.DrawRay(this._ray.origin, this._ray.direction * _distance, Color.yellow);
            if (Physics.Raycast(this._ray, out this._hitInfo, this._distance)) {
                Debug.Log(this._hitInfo.transform.gameObject.name.ToString());
                this.SelectObject(this._hitInfo);
            }
        }

        private void SelectObject(RaycastHit hitinfo) {
            if (Input.GetMouseButtonUp(0)) {
                if(hitinfo.transform.GetComponent<GameObject>().GetType() == typeof(Player)) {

                }
            }
        }
        #endregion
    }
}