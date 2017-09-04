namespace Player {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera_OLD : MonoBehaviour {

        public int _playerID = 0;

        public int PlayerID {
            get { return this._playerID; }
        }

        private Transform _transform;
        private Camera _camera;
        private bool _fixedUpdate = false;

        #region Movement
        private float _keyboardMoveSpeed = 10.0f;
        private float _screenEdgeSpeed = 10.0f;
        //private float _followSpeed = 10.0f;
        private float _rotationCameraSpeed = 120.0f;
        //private float _rotateAtPointSpeed = 10.0f;
        private float _panningSpeed = 10.0f;
        private float _mouseRotationSpeed = 10.0f;
        #endregion

        #region Height
        //private bool _autoHeight = true;
        private LayerMask _groundMask = -1;

        private float _maxHeight = 20.0f;
        private float _minHeight = 10.0f;
        private float _heightDamp = 7.0f;
        private float _keyboardZoomSensitivity = 5.0f;
        private float _scrollWheelZoomSensitivity = 25.0f;

        private float _zoomPos = 0.0f;
        #endregion

        #region MapLimits
        private bool _mapLimit = true;
        private float _limitX = 50.0f;
        private float _limitY = 50.0f;
        #endregion

        #region Target
        public Transform _targetFollowTransform;
        public Vector3 _targetOffset;

        public bool FollowingTarget {
            get { return this._targetFollowTransform != null; }
        }
        #endregion

        #region Input
        private bool _useScreenEdgeInput = true;
        private float _screenEdgeBoarder = 30.0f;

        private bool _useKeyboardInput = true;
        private string horizontalAxis = "Horizontal";
        private string verticleAxis = "Vertical";

        private bool _usePanning = true;
        private KeyCode _panningKey = KeyCode.Mouse2;

        private bool _useKeyboardZooming = true;
        private KeyCode _zoomIn = KeyCode.Z;
        private KeyCode _zoomOut = KeyCode.X;

        private bool _useScrollWheelZooming = true;
        private string _zoomingAxis = "Mouse ScrollWheel";

        private bool _useKeyboardRotation = true;
        private KeyCode _rotateKeyRight = KeyCode.E;
        private KeyCode _rotateKeyLeft = KeyCode.Q;

        private bool _useMouseRotation = true;
        private KeyCode _mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput {
            get { return _useKeyboardInput ? new Vector2(Input.GetAxis(this.horizontalAxis), Input.GetAxis(this.verticleAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel {
            get { return Input.GetAxis(this._zoomingAxis); }
        }

        private Vector2 MouseAxis {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

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

        #region Unity_Methods
        private void Start() {
            this._transform = this.transform;
            this._camera = this.GetComponent<Camera>() as Camera;;
        }

        private void Update() {
            if(!this._fixedUpdate && this._camera.enabled)
                this.CameraUpdate();
        }

        private void FixedUpdate() {
            if(this._fixedUpdate && this._camera.enabled)
                this.CameraUpdate();
        }
        #endregion

        #region Camera_Methods

        public void Init(int id) {
            this._playerID = id;
        }

        private void CameraUpdate() {
            if(this.FollowingTarget)
                this.FollowTarget();
            else
                this.MoveCamera();

            this.HeightCalculation();
            this.Rotation();
            this.LimitPosition();

        }

        private void FollowTarget() {

        }

        private void MoveCamera() {
            if(this._useKeyboardInput) {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= this._keyboardMoveSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, transform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this._transform.InverseTransformDirection(desiredMove);

                this.transform.Translate(desiredMove, Space.Self);
            }

            if(this._useScreenEdgeInput) {
                Vector3 desiredMove = new Vector3();

                Rect leftSide = new Rect(0.0f, 0.0f, this._screenEdgeBoarder, Screen.height);
                Rect rightSide = new Rect(Screen.width - this._screenEdgeBoarder, 0.0f, this._screenEdgeBoarder, Screen.height);
                Rect topSide = new Rect(0.0f, Screen.height - this._screenEdgeBoarder, Screen.width, this._screenEdgeBoarder);
                Rect downSide = new Rect(0.0f, 0.0f, Screen.width, this._screenEdgeBoarder);

                desiredMove.x = leftSide.Contains(this.MouseInput) ? -1 : rightSide.Contains(this.MouseInput) ? 1 : 0;
                desiredMove.z = topSide.Contains(this.MouseInput) ? -1 : downSide.Contains(this.MouseInput) ? 1 : 0;

                desiredMove *= this._screenEdgeSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, this.transform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this._transform.TransformDirection(desiredMove);

                this._transform.Translate(desiredMove, Space.Self);
            }


            if(this._usePanning && Input.GetKey(this._panningKey) && MouseAxis != Vector2.zero) {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0.0f, -MouseAxis.y);

                desiredMove *= this._panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, transform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this.transform.InverseTransformDirection(desiredMove);

                this._transform.Translate(desiredMove, Space.Self);
            }
        }

        private void HeightCalculation() {
            float distanceToGround = this.DistanceGround();

            if(this._useScrollWheelZooming)
                this._zoomPos += this.ScrollWheel * Time.deltaTime * this._scrollWheelZoomSensitivity;
            if(this._useKeyboardZooming)
                this._zoomPos += this.ZoomDirection * Time.deltaTime * this._keyboardZoomSensitivity;

            this._zoomPos = Mathf.Clamp01(this._zoomPos);

            float targetHeight = Mathf.Lerp(this._minHeight, this._maxHeight, this._zoomPos);
            float difference = 0.0f;

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            this._transform.position = Vector3.Lerp(this._transform.position, 
                                                    new Vector3(this._transform.position.x, targetHeight + difference, this._transform.position.z),
                                                    Time.deltaTime * this._heightDamp);
        }

        private void Rotation() {
            if(this._useKeyboardRotation)
                this.transform.Rotate(Vector3.up, this.RotationDirection * Time.deltaTime * this._rotationCameraSpeed, Space.World);

            if(this._useMouseRotation && Input.GetKey(this._mouseRotationKey))
                this._transform.Rotate(Vector3.up, -this.MouseAxis.x * Time.deltaTime * this._mouseRotationSpeed, Space.World);
        }

        private void LimitPosition() {
            if(!this._mapLimit)
                return;

            this._transform.position = new Vector3(Mathf.Clamp(this._transform.position.x, -this._limitX, 
                                                   this._limitX), this._transform.position.y, 
                                                   Mathf.Clamp(this._transform.position.z, -this._limitY, this._limitY));
        }

        public void SetTarget(Transform target) {
            this._targetFollowTransform = target;
        }

        public void ResetTarget() {
            this._targetFollowTransform = null;
        }

        private float DistanceGround() {
            Ray ray = new Ray(this._transform.position, Vector3.down);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, _groundMask.value))
                return (hit.point - this._transform.position).magnitude;

            return 0.0f;
        }

        #endregion

        #region PLAYER_CAMERA_STATIC
        public static GameObject CreateCamera(int id, string Name) {
            GameObject tempCamera;

            tempCamera = new GameObject(Name + (id+1).ToString());
            tempCamera.tag = "MainCamera";

            tempCamera.AddComponent<Camera>();
            tempCamera.AddComponent<GUILayer>();
            tempCamera.AddComponent<FlareLayer>();
            tempCamera.AddComponent<AudioListener>();
            tempCamera.AddComponent<PlayerCamera>();

            tempCamera.transform.position = new Vector3(0.0f, 20.0f, 0.0f);
            tempCamera.transform.rotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);

            tempCamera.GetComponent<PlayerCamera_OLD>()._playerID = id;

            Camera temp = tempCamera.GetComponent<Camera>() as Camera;

            if(id == 0)
                temp.enabled = true;
            else
                temp.enabled = false;

            return tempCamera;
        }
        #endregion

        #region DEBUG
        public void SwitchPlayers() {

        }
        #endregion
    }
}