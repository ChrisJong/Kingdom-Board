namespace Unit {

    using System;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(ClericUI))]
    public sealed class Cleric : UnitBase {

        #region VARIABLE
        [Header("CLERIC - SPECIAL")]
        [SerializeField] private bool _canHeal = true;

        [SerializeField] private float _healingRange = 0.0f;
        [SerializeField] private float _healingAmount = 0.0f;

        [Header("CLERIC - ANIMATION")]
        [SerializeField] private float _endOfCastClipTime = 0.542f;

        public bool CanHeal { get { return this._canHeal; } }

        public float HealingRange { get { return this._healingRange; } }
        public float HealingAmoumt { get { return this._healingAmount; } }
        #endregion

        #region CLASS
        public override void Setup() {

            this._canHeal = true;
            this._healingAmount = this._data.healingDamage;
            this._healingRange = this._data.healingRange;

            this._endOfCastClipTime = this._data.endOfCastClipTime;

            base.Setup();
        }

        public override void Init() {

            this._canHeal = true;

            this._healingAmount = this._data.healingDamage;
            this._healingRange = this._data.healingRange;

            base.Init();
        }

        public override void Return() {

            this._canHeal = false;

            base.Return();
        }

        public override void NewTurn() {

            this._canHeal = true;

            base.NewTurn();
        }

        public override bool SetTarget(IHasHealth target) {

            if(this.IsAlly(target)) {

                if(target.entityType == EntityType.STRUCTURE)
                    return false;

                if(!this._canAttack || !this._canHeal)
                    return false;

                if(this._currentTarget != null) {
                    this._previousTarget = this._currentTarget;
                    this.debugPreviousTarget = this._currentTarget as HasHealthBase;
                }
                this._currentTarget = target;
                this.debugCurrentTarget = target as HasHealthBase;

                this.unitUI.DrawRadius(Color.green, (this._healingRange + this._unitRadius));

                if(!this.TargetInRange()) {
                    if(this.CanMove) {
                        this.unitUI.EnableMovePathToTarget(this._currentTarget.position, this._healingRange);
                        return true;
                    } else
                        return false;
                } else {

                    if(this._currentPoint.HasValue) {
                        this._previousPoint = this._currentPoint;
                        this.debugPreviousPOint = this._currentPoint.Value;
                    }
                    this._currentPoint = null;
                    this.debugCurrentPoint = Vector3.zero;

                    this.unitUI.DisableMovePath();

                    return true;
                }
            }

            return base.SetTarget(target);
        }

        public override void InitiateTarget() {

            if(this.IsAlly(this._currentTarget)) {

                if(!this.CanHeal)
                    return;

                this._nextState = UnitState.SPECIAL;

                this.unitUI.MoveRadiusToOrigin();

                if(this._currentPoint.HasValue && this.CanMove) {
                    this.InitiateMove();
                    return;
                }

                if(this.TargetInRange()) {
                    this._previousState = this._currentState;
                    this._currentState = this._nextState;
                    this._nextState = UnitState.IDLE;
                    this.StartStateAnimation();
                } else {
                    this._previousState = this._currentState;
                    this._currentState = UnitState.IDLE;
                    this._nextState = UnitState.NONE;
                }

            } else {
                base.InitiateTarget();
            }
        }

        protected override bool TargetInRange() {

            if(this.IsAlly(this._currentTarget)) {
                float distance = Vector3.Distance(this.position, this._currentTarget.position);
                distance -= ((UnitBase)this._currentTarget).UnitRadius;

                //Debug.Log("Distance: " + distance.ToString());
                //Debug.Log("Unit Healing Range: " + (this._healingRange + this._unitRadius).ToString());

                if(distance > (this._healingRange + this._unitRadius))
                    return false;
                else
                    return true;

            } else
                return base.TargetInRange();
        }

        public override void ProjectileCollisionEvent() {
            if(this._currentState == UnitState.SPECIAL) {
                this.InternalHeal();
            } else {
                base.ProjectileCollisionEvent();
            }
        }
        #endregion

        #region HEALING
        public void Heal() {
            Debug.Log("HEALING UNIT");

            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_CLERIC_HEAL, this._currentTarget.position);
            if(this._projectilePrefab == null) {
                this._currentState = UnitState.SPECIAL;
                this.InternalHeal();
            } else {
                // NOTE: need to pass into a ID for the type of particle the end of the projectile should spawn.
                GameObject temp = Instantiate(this._projectilePrefab);
                Projectile tempProjjectile = temp.GetComponent<Projectile>() as Projectile;

                if(tempProjjectile == null)
                    tempProjjectile = temp.AddComponent<Projectile>();

                tempProjjectile.SetupTarget(this as IHasHealth, this._currentTarget, this._projectileReleasePoint.position, this._projectileSpeed);
            }
        }

        private void InternalHeal() {

            this._currentTarget.AddHealth(this._healingAmount);

            this._previousState = this._currentState;
            this._currentState = this._nextState;
            this._nextState = UnitState.NONE;
            this._canHeal = false;

            this._canAttack = false;
            this._hasStamina = false;
            this._currentEnergy = 0.0f;
            this.StopMoving();

            this.Controller.playerSelect.ChangeState(SelectionState.FREE);
        }
        #endregion

        #region ATTACK
        protected override void InternalAttack(float damage) {

            this._canHeal = false;

            base.InternalAttack(damage);
        }

        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_CLERIC_ATTACK, this._currentTarget.position);
        }
        #endregion

        #region ANIMATION
        public override void StartStateAnimation() {
            if(this._currentState == UnitState.SPECIAL) {
                this.LookAt(this._currentTarget.position);
                this.StopMoving();
                this._unitAnimator.Play("Cast");
            } else {
                base.StartStateAnimation();
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

    }
}