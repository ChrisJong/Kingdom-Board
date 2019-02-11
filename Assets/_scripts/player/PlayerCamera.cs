namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour {

        #region VARIABLE
        private bool _enableFixedUpdate = false;

        private Player _controller;
        private LayerMask _groundMask = 1 << 11;
        [SerializeField] private Transform _transform;
        private Camera _camera;
        public Camera mainCamera { get { return this._camera; } }

        // SPEED
        private float _keyboardSpeed = 25.0f;
        private float _rotationSpeed = 85.0f;

        // HEIGHT
        private float _currentHeight = 0.0f;
        private float _minHeight = 15.0f;
        private float _maxHeight = 20.0f;
        private float _heightDamp = 7.0f;

        // ZOOM
        private float _zoomDistance = 1.0f;
        private float _maxZoom = 2.25f;
        private float _minZoom = 0.5f;
        private float _cameraAngle = 55.0f;
        private float _scrollWheelZoomSens = 10.0f;

        // MAP LIMITS
        private bool _enableMapLimit = true;
        private float _limitX = 14.0f;
        private float _limitY = 20.0f;

        // INPUT CONTROLS
        #region INPUT CONTROLS
        private bool _enableKeyInput = true;
        private string _horizontalAxis = "Horizontal";
        private string _verticalAxis = "Vertical";

        private bool _enableKeyRotation = true;
        private KeyCode _rotateKeyRight = KeyCode.E;
        private KeyCode _rotateKeyLeft = KeyCode.Q;

        private bool _enableScrollWheelZoom = true;
        private string _zoomAxis = "Mouse ScrollWheel";
        private KeyCode _zoomIn = KeyCode.Z;
        private KeyCode _zoomOut = KeyCode.X;

        private Vector2 KeyMovementInput {
            get { return _enableKeyInput ? new Vector2(
                Input.GetAxis(this._horizontalAxis), 
                Input.GetAxis(this._verticalAxis)) : Vector2.zero; }
        }

        private float ScrollWheel {
            get { return Input.GetAxis(this._zoomAxis); }
        }

        private Vector2 MouseInput {
            get { return Input.mousePosition; }
        }

        private Vector2 MouseAxis {
            get { return new Vector2(
                Input.GetAxis("Mouse X"), 
                Input.GetAxis("Mouse Y")); }
        }
        #endregion

        private int ZoomDirection {
            get {
                bool zoomIn = Input.GetKey(this._zoomIn);
                bool zoomOut = Input.GetKey(this._zoomOut);

                if(zoomIn && zoomOut)
                    return 0;
                else if(!zoomIn && zoomOut)
                    return 1;
                else if(zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
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

        #endregion

        #region UNITY
        private void Start() {
            this._transform.rotation = Quaternion.Euler(this._cameraAngle, this._transform.eulerAngles.y, this._transform.eulerAngles.z);
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

        #region CLASS
        public void Init(Player controller) {
            this._controller = controller;
            this._transform = this.transform;
            this._camera = this.GetComponent<Camera>() as Camera;

            this._transform.rotation = Quaternion.Euler(this._cameraAngle, this._transform.eulerAngles.y, this._transform.eulerAngles.z);
        }

        private void CameraUpdate() {
            this.MoveCamera();
            //this.HeightCalculation();
            this.ZoomCamera();
            this.RotateCamera();
            this.LimitPosition();
        }

        private void MoveCamera() {
            if(this._enableKeyInput) {
                Vector3 desiredMove = new Vector3(KeyMovementInput.x, 0, KeyMovementInput.y);

                desiredMove *= this._keyboardSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, transform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this._transform.InverseTransformDirection(desiredMove);

                this.transform.Translate(desiredMove, Space.Self);
            }
        }

        private void HeightCalculation() {
            // NOTE: refactor needs to take into accoun the angle of the camera zooming in/out and the height of the camera to the ground.
            /*float distanceToGround = this.DistanceGround();

            if(this._enableScrollWheelZoom)
                this._currentHeight += this.ScrollWheel * Time.deltaTime * this._scrollWheelZoomSens;

            this._currentHeight = Mathf.Clamp01(this._currentHeight);

            float targetHeight = Mathf.Lerp(this._minHeight, this._maxHeight, this._currentHeight);
            float difference = 0.0f;

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;*/

            float targetHeight = Mathf.Lerp(this._minHeight, this._maxHeight, this._currentHeight);
            float difference = 0.0f;

            this._transform.position = Vector3.Lerp(this._transform.position,
                                                    new Vector3(this._transform.position.x, targetHeight + difference, this._transform.position.z),
                                                    Time.deltaTime * this._heightDamp);
        }

        private void ZoomCamera() {
            if(this._enableScrollWheelZoom) {
                if(this.ScrollWheel > 0) {
                    this._zoomDistance += this.ScrollWheel;
                    if(this._zoomDistance > this._maxZoom)
                        this._zoomDistance = this._maxZoom;
                    else
                        this.transform.Translate(0, 0, this.ScrollWheel * _scrollWheelZoomSens);
                } else {
                    this._zoomDistance += this.ScrollWheel;
                    if(this._zoomDistance < this._minZoom)
                        this._zoomDistance = this._minZoom;
                    else
                        this.transform.Translate(0, 0, this.ScrollWheel * _scrollWheelZoomSens);
                }
            }
        }

        private void RotateCamera() {
            if(this._enableKeyRotation)
                this.transform.Rotate(Vector3.up, this.RotationDirection * Time.deltaTime * this._rotationSpeed, Space.World);
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
                Debug.DrawRay(this._transform.position, Vector3.down * 1000.0f, Color.green);
                return (hit.point - this._transform.position).magnitude;
            }
            return 0.0f;
        }
        #endregion

        #region STATIC
        public static PlayerCamera CreateCamera(Player p, Transform startPoint) {
            GameObject tempCamera = new GameObject("Player" + (p.id+1).ToString().PadLeft(1) + "_CAMERA");

            tempCamera.AddComponent<Camera>();
            tempCamera.AddComponent<FlareLayer>();
            tempCamera.AddComponent<AudioListener>();
            p.playerSound.Init(tempCamera.AddComponent<AudioSource>());

            // NOTE: Used for IPointerHandlers for Shape Colliders, Doesn't actually work at the moment cause it collides with the Phyiscs Raycast in the Player Select Script.
            //tempCamera.AddComponent<UnityEngine.EventSystems.PhysicsRaycaster>();

            tempCamera.AddComponent<PlayerCamera>().Init(p);
            tempCamera.AddComponent<PlayerSelect>().Init(p);

            tempCamera.transform.parent = p.transform;

            if(startPoint.eulerAngles.y >= 180.0f)
                tempCamera.transform.position = new Vector3(startPoint.position.x, 20.0f, startPoint.position.z - 7.5f);
            else
                tempCamera.transform.position = new Vector3(startPoint.position.x, 20.0f, startPoint.position.z + 7.5f);

            //tempCamera.transform.position = new Vector3(startPoint.position.x, 20.0f, startPoint.position.z + 7.5f);
            tempCamera.transform.localRotation = Quaternion.Euler(0.0f, startPoint.eulerAngles.y + 180.0f, 0.0f);

            return tempCamera.GetComponent<PlayerCamera>() as PlayerCamera;
        }
        #endregion
    }
}