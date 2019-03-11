namespace KingdomBoard.Unit {

    using UnityEngine;

    using Enum;
    using Manager;

    [RequireComponent(typeof(AudioSource))]
    public class UnitSound : MonoBehaviour {

        #region VARIABLE

        [SerializeField] private UnitBase _unitBase = null;

        [SerializeField] private AudioSource _audioSource = null;

        [SerializeField] private AudioClip _currentClip = null;

        #endregion

        #region CLASS
        public void Setup(UnitBase unitBase) {
            this._unitBase = unitBase;

            if(this._audioSource == null) {
                if(this.GetComponent<AudioSource>() == null)
                    this._audioSource = this.gameObject.AddComponent<AudioSource>();
                else
                    this._audioSource = this.GetComponent<AudioSource>();
            }
            this._audioSource.playOnAwake = false;
        }

        public void Init() {
            
        }

        public void PlayClip() {

            if(this._currentClip == null) {
                Debug.LogError("There is no audio clip loaded into the current clip! " + this.ToString());
                throw new System.NullReferenceException("There is no audio clip loaded into the current clip! " + this.ToString());
            } else {
                if(this._audioSource.isPlaying)
                    this._audioSource.Stop();

                if(this._audioSource.clip != this._currentClip)
                    this._audioSource.clip = this._currentClip;

                this._audioSource.Play();
            }
        }

        public void StopClip() {
            if(this._audioSource.isPlaying)
                this._audioSource.Stop();

            this._currentClip = null;
        }

        public bool PlayAttack() {

            if(this.PlayAttack(this._unitBase.unitType))
                return true;

            if(this.PlayAttack(this._unitBase.classType))
                return true;

            Debug.LogWarning("No Attack Sound For The Type Of:" + this._unitBase.unitType.ToString() + " / " + this._unitBase.classType.ToString() );
            return false;
        }

        public bool PlayAttack(UnitClassType classType) {
            if(classType == UnitClassType.MAGIC) {

            } else if(classType == UnitClassType.MELEE) {

            } else if(classType == UnitClassType.RANGE) {
                this._currentClip = SoundManager.instance.rangeRelease;
            } else {
                Debug.LogWarning("No Attack Sound Found for The Type Of: " + classType.ToString());
            }

            if(this._currentClip == null)
                return false;

            this.PlayClip();
            return true;
        } 

        public bool PlayAttack(UnitType unitType) {
            switch(unitType) {
                case UnitType.MAGE:
                this._currentClip = SoundManager.instance.mageAttack;
                break;

                case UnitType.CLERIC:
                this._currentClip = SoundManager.instance.clericAttack;
                break;

                case UnitType.ARCHER:
                this._currentClip = SoundManager.instance.rangeRelease;
                break;

                case UnitType.LONGBOW:
                this._currentClip = SoundManager.instance.rangeRelease;
                break;

                case UnitType.CROSSBOW:
                this._currentClip = SoundManager.instance.rangeRelease;
                break;

                default:
                Debug.LogWarning("No Attack Sound Found for The Type Of: " + unitType.ToString());
                break;
            }

            if(this._currentClip == null)
                return false;

            this.PlayClip();
            return true;
        }

        public bool PlaySpecial() {

            if(this.PlaySpecial(this._unitBase.unitType))
                return true;

            Debug.LogWarning("No Special Sound Found For The Type: " + this._unitBase.unitType.ToString());
            return false;
        }

        public bool PlaySpecial(UnitType unitType) {
            switch(unitType) {
                case UnitType.CLERIC:
                this._currentClip = SoundManager.instance.clericHeal;
                break;

                case UnitType.WIZARD:
                this._currentClip = SoundManager.instance.wizardExplosion;
                break;

                default:
                Debug.LogWarning("No Special Sound found For The Type Of: " + unitType.ToString());
                break;
            }

            if(this._currentClip == null)
                return false;

            this.PlayClip();
            return true;
        }

        public bool PlayImpact() {

            IUnit unitBase = (this._unitBase.LastAttacker.entityType == EntityType.UNIT ? this._unitBase.LastAttacker as IUnit : null);

            if(unitBase != null) {
                UnitClassType classType = unitBase.classType;
                UnitType unitType = unitBase.unitType;

                if(this.PlayImpact(classType))
                    return true;

                if(this.PlayImpact(unitType))
                    return true;
            }

            Debug.LogWarning("Unit Base Is Null!");
            return false;
        } 

        public bool PlayImpact(UnitType enemyUnitType) {

            int count = 0;

            switch(enemyUnitType) {
                case UnitType.ARCHER:
                count = SoundManager.instance.rangeImpact.Count;
                this._currentClip = SoundManager.instance.rangeImpact[Random.Range(0, count - 1)];                
                break;

                case UnitType.LONGBOW:
                count = SoundManager.instance.rangeImpact.Count;
                this._currentClip = SoundManager.instance.rangeImpact[Random.Range(0, count - 1)];
                break;

                case UnitType.CROSSBOW:
                count = SoundManager.instance.rangeImpact.Count;
                this._currentClip = SoundManager.instance.rangeImpact[Random.Range(0, count - 1)];
                break;

                case UnitType.WARRIOR:
                count = SoundManager.instance.meleeImpact.Count;
                this._currentClip = SoundManager.instance.meleeImpact[Random.Range(0, count - 1)];
                break;

                case UnitType.KNIGHT:
                count = SoundManager.instance.meleeImpact.Count;
                this._currentClip = SoundManager.instance.meleeImpact[Random.Range(0, count - 1)];
                break;

                case UnitType.GUARDIAN:
                count = SoundManager.instance.meleeImpact.Count;
                this._currentClip = SoundManager.instance.meleeImpact[Random.Range(0, count - 1)];
                break;

                default:
                Debug.LogWarning("No Impact Sound For The Type Of:" + enemyUnitType.ToString());
                break;
            }

            if(this._currentClip == null)
                return false;

            this.PlayClip();
            return true;
        }

        public bool PlayImpact(UnitClassType enemyClassType) {
            if(enemyClassType == UnitClassType.MAGIC) {

            } else if(enemyClassType == UnitClassType.MELEE) {

                int count = SoundManager.instance.meleeImpact.Count;
                this._currentClip = SoundManager.instance.meleeImpact[Random.Range(0, count - 1)];

            } else if(enemyClassType == UnitClassType.RANGE) {

                int count = SoundManager.instance.rangeImpact.Count;
                this._currentClip = SoundManager.instance.rangeImpact[Random.Range(0, count - 1)];

            } else {
                Debug.LogWarning("No Impact Sound For The Type Of:" + enemyClassType.ToString());
                return false;
            }

            if(this._currentClip == null)
                return false;

            this.PlayClip();
            return true;
        }

        public bool PlayDeath() {

            int count = SoundManager.instance.unitDeath.Count;
            this._currentClip = SoundManager.instance.unitDeath[Random.Range(0, count - 1)];

            if(this._currentClip == null) {
                Debug.LogError("No Death Sounds For Unit Death!");
                return false;
            }

            this.PlayClip();
            return true;
        }

        public AudioClip GetDeathSoundclip() {
            int count = SoundManager.instance.unitDeath.Count;
            AudioClip temp = SoundManager.instance.unitDeath[Random.Range(0, count - 1)];

            if(temp == null) {
                Debug.LogError("No Death Sounds For Unit Death!");
                throw new System.NullReferenceException("No Death Sounds For Unit Death!");
            }

            return temp;
        }
        #endregion
    }
}