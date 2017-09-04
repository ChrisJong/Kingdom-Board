namespace Player
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        #region VARIABLES

        private int _playerID = 0;
        public int PlayerID {
            get { return this._playerID; }
        }

        private LayerMask _groundMask = 1 << 11;
        private Transform _transform;
        private Camera _camera;

        private bool _enableFixedUpdate = false;

        #region MOVEMENT_SPEEDS
        private float _keyboardSpeed = 10.0f;
        private float _rotationSpeed = 50.0f;
        #endregion

        #region HEIGHT
        public float currentHeight = 0.0f;
        private float _maxHeight = 20.0f;
        private float _minHeight = 10.0f;
        private float _heightDamp = 7.0f;
        private float _scrollWheelZoomSensitivity = 25.0f;
        #endregion

        #region MAP_LIMITS
        private bool _enableMapLimit = true;
        private float _limitX = 50.0f;
        private float _limitY = 50.0f;
        #endregion

        #region INPUT_CONTROLS
        private bool _enableKeyboardInput = true;
        private string _horizontalAxis = "Horizontal";
        private string _verticalAxis = "Vertical";

        private bool _enableKeyboardRotation = true;
        private KeyCode _rotateKeyRight = KeyCode.E;
        private KeyCode _rotateKeyLeft = KeyCode.Q;

        private bool _enableScrollWheelZooming = true;
        private string _zoomingAxis = "Mouse ScrollWheel";

        private Vector2 KeyboardMovementInput {
            get { return _enableKeyboardInput ? new Vector2(Input.GetAxis(this._horizontalAxis), Input.GetAxis(this._verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MousePositionInput {
            get { return Input.mousePosition; }
        }

        private Vector2 MouseAxis {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int RotationDirection {
            get {
                bool rotateRight = Input.GetKey(this._rotateKeyRight);
                bool rotateLeft = Input.GetKey(this._rotateKeyLeft);

                if(rotateLeft && rotateRight)
                    return 0;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else
                    return 0;
            }
        }

        private float ScrollWheel {
            get { return Input.GetAxis(this._zoomingAxis); }
        }
        #endregion

        #endregion

        #region UNITY_METHODS
        private void Awake() {
            this._transform = this.transform;
            this._camera = this.GetComponent<Camera>() as Camera;
        }

        private void Update() {
            if(!this._enableFixedUpdate && this._camera.enabled)
                this.CameraUpdate();
                
        }

        private void FixedUpdate() {
            if(this._enableFixedUpdate && this._camera.enabled)
                this.CameraUpdate();
        }
        #endregion

        #region CAMERA_METHODS

        private void CameraUpdate() {
            this.MoveCamera();
            this.HeightCalculation();
            this.Rotation();
            this.LimitPosition();
        }

        private void MoveCamera() {
            if(this._enableKeyboardInput) {
                Vector3 desiredMove = new Vector3(KeyboardMovementInput.x, 0, KeyboardMovementInput.y);

                desiredMove *= this._keyboardSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, transform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this._transform.InverseTransformDirection(desiredMove);

                this.transform.Translate(desiredMove, Space.Self);
            }
        }

        private void Rotation() {
            if(this._enableKeyboardRotation)
                this.transform.Rotate(Vector3.up, this.RotationDirection * Time.deltaTime * this._rotationSpeed, Space.World);
        }

        private void HeightCalculation() {
            float distanceToGround = this.DistanceGround();

            if(this._enableScrollWheelZooming)
                this.currentHeight += this.ScrollWheel * Time.deltaTime * this._scrollWheelZoomSensitivity;

            this.currentHeight = Mathf.Clamp01(this.currentHeight);

            float targetHeight = Mathf.Lerp(this._minHeight, this._maxHeight, this.currentHeight);
            float difference = 0.0f;

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            this._transform.position = Vector3.Lerp(this._transform.position,
                                                    new Vector3(this._transform.position.x, targetHeight + difference, this._transform.position.z),
                                                    Time.deltaTime * this._heightDamp);
        }

        private void LimitPosition() {
            if(!this._enableMapLimit)
                return;

            this._transform.position = new Vector3(Mathf.Clamp(this._transform.position.x, -this._limitX,
                                                   this._limitX), this._transform.position.y,
                                                   Mathf.Clamp(this._transform.position.z, -this._limitY, this._limitY));
        }

        private float DistanceGround() {
            Ray ray = new Ray(this._transform.position, Vector3.down);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit, 20.0f, this._groundMask.value)) {
                Debug.DrawRay(this._transform.position, Vector3.down * 20.0f, Color.blue);
                Debug.Log(((hit.point - this._transform.position).magnitude).ToString());
                return (hit.point - this._transform.position).magnitude;
            }
            return 0.0f;
        }

        #endregion
    }
}