namespace KingdomBoard.Player {

    using UnityEngine;

    public class PlayerCamera : MonoBehaviour {

        #region VARIABLE_CLEAN
        [SerializeField] private Player _controller;

        [SerializeField] private Camera _camera;

        [SerializeField] private Transform _cameraTransform = null;

        public Camera MainCamera { get { return this._camera; } }

        public Transform MainCameraTransform { get { return this._cameraTransform; } }
        #endregion

        #region VARIABLE
        private bool _enableFixedUpdate = false;

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
        private float _limitX = 25.0f;
        private float _limitZ = 25.0f;

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
            this._cameraTransform.rotation = Quaternion.Euler(this._cameraAngle, this._cameraTransform.eulerAngles.y, this._cameraTransform.eulerAngles.z);
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
        public void Init(Player controller, Transform spawnLocation) {
            this._controller = controller;

            this._camera = this.transform.Find("Camera").GetComponent<Camera>();
            if(this._camera == null) {
                GameObject tempCamera = new GameObject("Camera");
                this._camera = tempCamera.AddComponent<Camera>();
                tempCamera.AddComponent<AudioListener>();
                tempCamera.AddComponent<AudioSource>();
                tempCamera.transform.parent = this._controller.transform;
            }
            this._cameraTransform = this._camera.transform;

            if(spawnLocation.eulerAngles.y >= 180.0f)
                this._camera.transform.transform.localPosition = new Vector3(spawnLocation.position.x, 20.0f, spawnLocation.position.z - 7.5f);
            else
                this._camera.transform.transform.localPosition = new Vector3(spawnLocation.position.x, 20.0f, spawnLocation.position.z + 7.5f);

            this._cameraTransform.localRotation = Quaternion.Euler(this._cameraAngle, spawnLocation.eulerAngles.y + 180.0f, 0.0f);

            this._camera.gameObject.SetActive(false);
        }

        private void CameraUpdate() {

            if(this._controller.CurrentState == Enum.PlayerState.ATTACKING || this._controller.CurrentState == Enum.PlayerState.DEFENDING) {

                // For Debugging.
                if(Manager.GameManager.instance.PlayerInView != this._controller)
                    return;

                this.MoveCamera();
                //this.HeightCalculation();
                this.ZoomCamera();
                this.RotateCamera();
                this.LimitPosition();
            }
        }

        private void MoveCamera() {
            if(this._enableKeyInput) {
                Vector3 desiredMove = new Vector3(KeyMovementInput.x, 0, KeyMovementInput.y);

                desiredMove *= this._keyboardSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0.0f, this._cameraTransform.eulerAngles.y, 0.0f)) * desiredMove;
                desiredMove = this._cameraTransform.InverseTransformDirection(desiredMove);

                this._cameraTransform.Translate(desiredMove, Space.Self);
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

            this._cameraTransform.position = Vector3.Lerp(this._cameraTransform.position,
                                                    new Vector3(this._cameraTransform.position.x, targetHeight + difference, this._cameraTransform.position.z),
                                                    Time.deltaTime * this._heightDamp);
        }

        private void ZoomCamera() {
            if(this._enableScrollWheelZoom) {
                if(this.ScrollWheel > 0) {
                    this._zoomDistance += this.ScrollWheel;
                    if(this._zoomDistance > this._maxZoom)
                        this._zoomDistance = this._maxZoom;
                    else
                        this._cameraTransform.Translate(0, 0, this.ScrollWheel * _scrollWheelZoomSens);
                } else {
                    this._zoomDistance += this.ScrollWheel;
                    if(this._zoomDistance < this._minZoom)
                        this._zoomDistance = this._minZoom;
                    else
                        this._cameraTransform.Translate(0, 0, this.ScrollWheel * _scrollWheelZoomSens);
                }
            }
        }

        private void RotateCamera() {
            if(this._enableKeyRotation)
                this._cameraTransform.Rotate(Vector3.up, this.RotationDirection * Time.deltaTime * this._rotationSpeed, Space.World);
        }

        private void LimitPosition() {
            if(!this._enableMapLimit)
                return;

            this._cameraTransform.position = new Vector3(Mathf.Clamp(this._cameraTransform.position.x, -this._limitX,
                                                   this._limitX), this._cameraTransform.position.y,
                                                   Mathf.Clamp(this._cameraTransform.position.z, -this._limitZ, this._limitZ));
        }

        private float DistanceGround() {
            Ray ray = new Ray(this._cameraTransform.position, Vector3.down);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 20.0f, Constants.GlobalSettings.LayerValues.groundLayer)) {
                Debug.DrawRay(this._cameraTransform.position, Vector3.down * 1000.0f, Color.green);
                return (hit.point - this._cameraTransform.position).magnitude;
            }
            return 0.0f;
        }
        #endregion
    }
}