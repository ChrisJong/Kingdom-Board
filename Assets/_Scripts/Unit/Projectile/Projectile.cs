namespace Unit {

    using UnityEngine;

    using Enum;
    using Helpers;
    using Manager;
    using Particle;

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour {
        #region VARIABLE
        [SerializeField] private ParticleSystem _particleSystem;

        private IHasHealth _origin;
        private IHasHealth _target;
        private Vector3 _targetPosition;

        private Collider _targetCollider;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private Transform _transform;

        private bool _startMoving = false;
        private float _speed;
        private float _distanceCovered = 0;
        private float _distanceOffset = 0.5f;
        #endregion

        #region UNITY
        private void Awake() {
            foreach(Collider col in this.transform.GetComponents(typeof(Collider))) {
                this._collider = col;
            }

            this._rigidbody = this.transform.GetComponent<Rigidbody>() as Rigidbody;
            this._rigidbody.isKinematic = true;
            this._collider.isTrigger = true;

            this._transform = this.transform;
        }

        void OnTriggerEnter(Collider other) {
            IHasHealth otherHasHealth = other.GetEntity<IHasHealth>();

            if(otherHasHealth == null)
                return;

            if(otherHasHealth != this._target)
                return;
            else {
                Debug.Log("Hit Target");
                this.FinishCollision();
            }
        }

        private void Update() {
            this.UpdateProjectile();
        }
        #endregion

        #region CLASS
        public void SetupTarget(IHasHealth origin, IHasHealth target, Vector3 releasePoint, float speed) {
            //UnityEditor.EditorApplication.isPaused = true;

            this._targetCollider = target.transform.GetComponent<Collider>();
            if(this._targetCollider == null)
                return;

            if(this._targetCollider is CapsuleCollider)
                this._targetPosition = new Vector3(target.position.x, target.position.y + ((CapsuleCollider)this._targetCollider).center.y, target.position.z);
            else if(this._targetCollider is BoxCollider)
                this._targetPosition = new Vector3(target.position.x, target.position.y + ((BoxCollider)this._targetCollider).center.y, target.position.z);
            else
                this._targetPosition = target.position;

            this._origin = origin;
            this._target = target;
            this._speed = speed;

            this.transform.position = releasePoint;
            this.transform.LookAt(this._targetPosition, Vector3.forward);
            //float y = this.transform.eulerAngles.y;
            //this.transform.eulerAngles = new Vector3(0.0f, y);

            this._startMoving = true;
            UnityEditor.EditorApplication.isPaused = true;
        }

        protected virtual void UpdateProjectile() {
            if(this._startMoving) {
                transform.Translate(Vector3.forward * (this._speed * Time.deltaTime), Space.Self);
                this._distanceCovered = Vector3.Distance(this._transform.position, this._target.position);

                if(this._distanceCovered <= 0.0f + this._distanceOffset) {
                    this.FinishCollision();
                }
            }
        }

        private void FinishCollision() {
            Debug.Log(this._target.gameObject.name + " Has Been Hit");
            this._origin.gameObject.GetComponent<UnitBase>().ProjectileCollisionEvent();

            if(this._particleSystem != null) {
                this._particleSystem.gameObject.AddComponent<ProjectileParticle>();
                this._particleSystem.transform.parent = null;
            }

            Destroy(this.gameObject);
        }
        #endregion
    }
}