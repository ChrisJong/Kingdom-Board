namespace Unit {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class UnitSound : MonoBehaviour {

        #region VARIABLE

        [SerializeField] private UnitBase _unitBase = null;

        [SerializeField] private AudioSource _audioSource = null;

        #endregion

        #region CLASS
        public void Setup(UnitBase unitBase) {
            this._unitBase = unitBase;

            this._audioSource = this.GetComponent<AudioSource>();
            if(this._audioSource == null)
                this._audioSource = this.gameObject.AddComponent<AudioSource>();
        }

        public void Init() {
            
        }

        #endregion
    }
}