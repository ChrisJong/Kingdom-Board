namespace Unit {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Helpers;
    using Enum;
    using Utility;

    using System;

    [RequireComponent(typeof(AudioSource))]
    public class UnitDeath : ObjectPoolBase, IUnitDeath {

        #region VARIABLE

        [SerializeField] private bool _isSetup = false;

        [SerializeField] private int _turnCounter = 0;

        [Space]
        [SerializeField] private UnitType _unitType = UnitType.NONE;

        [Space]
        [SerializeField] private AudioSource _audioSource = null;

        [SerializeField] private List<Transform> _transforms = new List<Transform>();
        [SerializeField] private List<Rigidbody> _rigidbodies = new List<Rigidbody>();

        public bool IsSetup { get { return this._isSetup; } }

        public int TurnCounter { get { return this._turnCounter; } }

        public UnitType unitType { get { return this._unitType; } set { this._unitType = value; } }
        #endregion

        #region CLASS
        public void Setup() {
            this._isSetup = true;

            this._audioSource = this.GetComponent<AudioSource>();
            this._audioSource.playOnAwake = false;

            this._transforms.Clear();
            this._rigidbodies.Clear();

            this._transforms.AddRange(this.GetComponentsInChildren<Transform>());
            this._rigidbodies.AddRange(this.GetComponentsInChildren<Rigidbody>());
        }

        public void Return() {
            foreach(Transform t in this._transforms) {
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
            }
        }

        public void Init(Color color, Vector3 eDirection, Vector3 ePoseition, float eForce, int counter) {
            this._turnCounter = counter;

            this.gameObject.ColorRenderers(color);

            this._audioSource.clip = Manager.SoundManager.instance.GetUnitDeath();
            this._audioSource.Play();

            foreach(Rigidbody rb in this._rigidbodies)
                rb.AddForceAtPosition(eDirection * eForce, ePoseition);
        }
        #endregion

        public void Countdown() {
            this._turnCounter -= 1;
            Debug.Log(this.name + ": " + this._turnCounter.ToString());
        }
    }
}