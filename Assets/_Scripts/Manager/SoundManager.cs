namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Extension;

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : SingletonMono<SoundManager> {

        #region VARIABLE
        [SerializeField] private AudioSource _audioSource = null;

        // Music
        [Header("Music")]
        public AudioClip main = null;

        // Player
        [Header("Player")]
        public AudioClip attackPhase = null;
        public AudioClip defencePhase = null;
        public List<AudioClip> bookOpen = new List<AudioClip>();
        public AudioClip cardTurn = null;
        public AudioClip victory = null;
        public AudioClip defeat = null;

        // Unit
        [Header("Unit")]
        public List<AudioClip> unitDeath = new List<AudioClip>();
        public AudioClip rangeRelease = null;
        public AudioClip clericHeal = null;
        public AudioClip clericAttack = null;
        public AudioClip mageAttack = null;
        public AudioClip wizardExplosion = null;
        public List<AudioClip> meleeImpact = new List<AudioClip>();
        public List<AudioClip> rangeImpact = new List<AudioClip>();

        public AudioSource audioSource { get { return this._audioSource; } }
        #endregion

        #region UNITY
        private void Start() {
            this.Init();
        }
        #endregion

        #region CLASS
        public override void Init() {

            if(this._audioSource == null) {
                if(this.GetComponent<AudioSource>() == null)
                    this._audioSource = this.gameObject.AddComponent<AudioSource>();
                else
                    this._audioSource = this.GetComponent<AudioSource>();
            }
            this._audioSource.playOnAwake = false;
        }

        public AudioClip GetUnitClassImpact(UnitClassType classType) {
            AudioClip temp = null;
            int index = 0;

            switch(classType) {
                case UnitClassType.MAGIC:
                    
                break;

                case UnitClassType.MELEE:
                index = Random.Range(0, this.meleeImpact.Count - 1);
                temp = this.meleeImpact[index];
                break;

                case UnitClassType.RANGE:
                index = Random.Range(0, this.rangeImpact.Count - 1);
                temp = this.rangeImpact[index];
                break;
            }

            if(temp == null)
                throw new System.NullReferenceException("Null Reference Found: There Is no impact audio Clip For Class Type of " + classType.ToString());

            return temp;
        }

        public AudioClip GetUnitTypeImpact(UnitType unitTtype) {
            AudioClip temp = null;

            return temp;
        }

        public AudioClip GetUnitClassAttack(UnitClassType classType) {
            AudioClip temp = null;

            if(classType == UnitClassType.RANGE) {
                temp = this.rangeRelease;
            }

            if(temp == null)
                throw new System.NullReferenceException("No Class Attack Sound for Class Type Of " + classType.ToString());

            return temp;
        }

        public AudioClip GetUnitTypeAttack(UnitType unitType) {
            AudioClip temp = null;

            switch(unitType) {
                case UnitType.MAGE:
                temp = this.mageAttack;
                break;

                case UnitType.CLERIC:
                temp = this.clericAttack;
                break;

                case UnitType.WIZARD:
                temp = this.wizardExplosion;
                break;
            }

            if(temp == null)
                throw new System.NullReferenceException("No Attack Sound for Unit Type of " + unitType.ToString());

            return temp;
        }

        public AudioClip GetUnitSpecial(UnitType unitType) {
            AudioClip temp = null;

            switch(unitType) {
                case UnitType.CLERIC:
                temp = this.clericHeal;
                break;
            }

            if(temp == null)
                throw new System.NullReferenceException("Couldn't Find Special Sound For The Unit " + unitType.ToString());

            return temp;
        }
        #endregion
    }
}