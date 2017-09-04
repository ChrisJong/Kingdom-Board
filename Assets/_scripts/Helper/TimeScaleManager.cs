namespace Helper {
    using System;
    using UnityEngine;

    public class TimeScaleManager : Extension.SingletonMono<TimeScaleManager> {

        [SerializeField, Range(1f, 128f)]
        private float _maxTimeScale = 16.0f;
        [SerializeField, Range(0.0001f, 1f)]
        private float _minTimeScale = 0.0625f;

        [SerializeField]
        private bool _displayTime = true;

        [SerializeField, Tooltip("The size of the time GUI box.")]
        private Vector2 _guiSize = new Vector2(150.0f, 40.0f);

        #region UNITY_METHODS
        private void Update() {
            if(Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus)) {
                // increase time scale (up to max) on plus key
                if(Time.timeScale < this._maxTimeScale) {
                    Time.timeScale *= 2f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }
            } else if(Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) {
                // decrease time scale (down to min) on minus key
                if(Time.timeScale > this._minTimeScale) {
                    Time.timeScale *= 0.5f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }
            }
        }

        private void OnGUI() {
            if(this._displayTime) {
                GUI.Box(new Rect((Screen.width * 0.5f) - (this._guiSize.x * 0.5f), 5f, this._guiSize.x, this._guiSize.y),
                    string.Concat("Time Scale: ", Time.timeScale.ToString("F2"), "x\n", "Time Elapsed: ", Time.timeSinceLevelLoad.ToString("F0"), "s"));
            }
        }

        private void OnEnable() {
            Time.timeScale = 1.0f;
        }

        private void OnDisable() {
            Time.timeScale = 1.0f;
        }
        #endregion

        #region METHODS
        public override void Init() {
            throw new NotImplementedException();
        }
        #endregion
    }
}