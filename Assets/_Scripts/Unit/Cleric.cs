namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;

    [RequireComponent(typeof(ClericUI))]
    public sealed class Cleric : UnitBase {

        #region 
        [Header("CLERIC - HEALING")]
        [SerializeField] private bool _canHeal = true;
        [SerializeField, Range(1.0f, 50.0f)] private float _healingRadius = 10.0f;
        [SerializeField, Range(1.0f, 100.0f)] private float _healingAmount = 5.0f;

        public bool canHeal { get { return this._canHeal; } }
        public float healingRadius { get { return this._healingRadius; } }
        public float healingAmoumt { get { return this._healingAmount; } }

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        [Header("CLERIC - ANIMATION")]
        [SerializeField] private float _endOfCastClipTime = 0.542f;
        #endregion

        #region UNITY
        protected override void OnEnable() {
            this._canHeal = true;

            base.OnEnable();
        }
        #endregion

        #region CLASS
        public override void NewTurn() {
            base.NewTurn();
            this._canHeal = true;
        }

        public override void ProjectileCollisionEvent() {
            if(this._unitState == UnitState.HEAL_ANIMATION) {
                this._unitState = UnitState.HEAL;
                this.InternalHeal();
            } else {
                base.ProjectileCollisionEvent();
            }
        }

        protected override void CheckStandbyState(out bool value) {
            base.CheckStandbyState(out value);

            if(this._unitState == UnitState.HEAL_STANDBY) {
                if(this.IsEnemy(this._currentTarget)) {
                    this._currentTarget = null;
                    this._previousTarget = null;

                    value = false;
                    return;
                }

                bool targetInRange = false;

                float distance = Vector3.Distance(this.position, this._currentTarget.position);
                if(distance + this._unitRadius < this._healingRadius)
                    targetInRange = true;

                if(!targetInRange) {
                    Collider[] hits = Physics.OverlapSphere(this.position, this._healingRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
                    for(int i = 0; i < hits.Length; i++) {
                        IHasHealth hitHasHealth = hits[i].GetEntity<IHasHealth>();

                        if(hitHasHealth == null)
                            continue;

                        if(hitHasHealth != this._currentTarget)
                            continue;
                        else {
                            targetInRange = true;
                            break;
                        }
                    }
                }

                if(targetInRange) {
                    this.StartStateAnimation();
                } else {
                    value = targetInRange;
                    return;
                }
            }

            value = true;
        }

        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_CLERIC_ATTACK, this._currentTarget.position);
        }

        /////////////////
        //// HEALING ////
        /////////////////
        #region HEALING
        public void FinishHealing() {
            this._canHeal = false;
        }

        public void Heal() {
            Debug.Log("HEALING UNIT");
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_CLERIC_HEAL, this._currentTarget.position);
            if(this._projectilePrefan == null) {
                this._unitState = UnitState.HEAL;
                ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_CLERIC_HEAL, this._currentTarget.position);
                this.InternalHeal();
            } else {
                // NOTE: need to pass into a ID for the type of particle the end of the projectile should spawn.
                GameObject temp = Instantiate(this._projectilePrefan);
                Projectile tempProjjectile = temp.GetComponent<Projectile>() as Projectile;

                if(tempProjjectile == null)
                    temp.AddComponent<Projectile>();
                else
                    tempProjjectile.SetupTarget(this as IHasHealth, this._currentTarget, this._projectileReleasePoint.position, this._projectileSpeed);
            }
        }

        private void InternalHeal() {
            this._currentTarget.AddHealth(this._healingAmount);

            ((UI.ClericUI)this.uiBase).FinishHeal();
            this._unitState = UnitState.IDLE;
            this._canHeal = false;
        }
        #endregion

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        #region ANIMATION
        public override void StartStateAnimation() {
            base.StartStateAnimation();

            if(this._unitState == UnitState.HEAL_STANDBY) {
                this._unitState = UnitState.HEAL_ANIMATION;
                this._unitAnimator.Play("Cast");
            }
        }

        protected override void SetupEventAnimation() {
            base.SetupEventAnimation();

            AnimationEvent animEvent = new AnimationEvent();
            AnimationClip animClip = new AnimationClip();

            if(this._endOfCastClipTime <= 0.0f)
                throw new ArgumentException("End Of Animation Clip Time CAN NOT be set to 0 seconds");

            animEvent.time = this._endOfCastClipTime;
            animEvent.functionName = "Heal";

            foreach(AnimationClip clip in this._unitAnimator.runtimeAnimatorController.animationClips) {
                if(clip.name.Contains("Cast")) {
                    /*foreach(AnimationEvent evt in clip.events) {
                        Debug.Log("Event Cast Time: " + evt.time);
                        UnityEditor.EditorApplication.isPaused = true;
                    }*/
                    animClip = clip;
                    break;
                }
            }

            animClip.AddEvent(animEvent);
        }
        #endregion
        #endregion
    }
}