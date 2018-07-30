﻿namespace Unit {

    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {

        public GameObject projectileTarget;
        public GameObject projectileOrigin;

        public Collider projectileCollider;
        public Rigidbody projectileRigidbody;

        [SerializeField]
        private float _projectileSpeed;
        private bool _startMoving = false;
        //private float _distanceCovered = 0;

        #region UNITY
        private void Awake() {
            foreach(Collider col in this.transform.GetComponents(typeof(Collider))) {
                this.projectileCollider = col;
            }

            this.projectileRigidbody = this.transform.GetComponent<Rigidbody>() as Rigidbody;
            this.projectileRigidbody.isKinematic = true;
            this.projectileCollider.isTrigger = true;
        }

        void OnTriggerEnter(Collider other) {
            ///Debug.Log(other.transform.name);
            if(other.gameObject == this.projectileTarget) {
                Debug.Log(other.name + " Has Been Hit");
                this.projectileOrigin.GetComponent<UnitBase>().ProjectileAttack();
                Destroy(this.gameObject);
            }
        }

        private void Update() {

            // Homing Style Projectile.
            /*if(this._startMoving){
                if(this.projectileTarget != null) {
                    var relativePos = this.projectileTarget.transform.position - transform.position;
                    var rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f);
                }

                transform.Translate(0, 0, this._projectileSpeed * Time.deltaTime, Space.Self);
            }*/

            /*if(this._startMoving) {
                this._distanceCovered = Vector3.Distance(this.transform.position, this.projectileTarget.transform.position);
                this.transform.position += Vector3.MoveTowards(this.transform.position, this.projectileTarget.transform.position, Time.deltaTime * this._projectileSpeed);

                if(this._distanceCovered <= 1.0f) {
                    Debug.Log("Has Been Hit");
                    Destroy(this.gameObject);
                }
            }*/

            if(this._startMoving) {
                this.transform.position = Vector3.MoveTowards(this.transform.position, this.projectileTarget.transform.position, this._projectileSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region CLASS
        public void SetupTarget(GameObject origin, GameObject target, Vector3 pos, float speed) {
            this.projectileOrigin = origin;
            this.projectileTarget = target;
            this._projectileSpeed = speed;
            this.transform.LookAt(target.transform);
            this.transform.position = origin.transform.position;
            //this._distanceCovered = Vector3.Distance(this.transform.position, target.transform.position);

            transform.position = new Vector3(transform.position.x, this.transform.position.y +  2.5f, this.transform.position.z);

            this._startMoving = true;
        }
        #endregion
    }
}