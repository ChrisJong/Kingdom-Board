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
        private ClericUI _clericUI = null;

        [Header("CLERIC - HEALING")]
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

            this._endOfCastClipTime = this._data.endOfCastClipTime;

            base.Setup();

            this._clericUI = this.uiBase as ClericUI;
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

            if(!this._canAttack && !this._canHeal)
                return false;

            if(this._currentTarget != null)
                this._previousTarget = this. _currentTarget;

            this._currentTarget = target;

            float distance = 0.0f;
            if(target.entityType == EntityType.UNIT)
                distance = Vector3.Distance(this.position, this._currentTarget.position) - (this._unitRadius - ((UnitBase)target).UnitRadius);
            else
                distance = Vector3.Distance(this.position, Utils.ClosesPointToBounds(((Structure.StructureBase)this._currentTarget).ColliderBounds, this.position));

            if(this.IsAlly(target)) { // Heal the target.
                distance = Vector3.Distance(this.position, target.position);
                this.unitUI.DrawRadius(Color.green, this._healingRange);

                if(distance > (this._healingRange + this._unitRadius)) { // Can Move To the Target.
                    this.unitUI.EnableMovePathToTarget(target.position, this._healingRange);
                } else {

                    if(this._currentPoint.HasValue) {
                        this._previousPoint = this._currentPoint;
                        this.debugPreviousPOint = this._previousPoint.Value;
                    }

                    this._currentPoint = null;
                    this.debugCurrentPoint = Vector3.zero;

                    this.unitUI.DisableMovePath();
                }

            } else { // Attack the target.
                this.unitUI.EnableAttackRadius();

                if(distance > (this._attackRange + this._unitRadius)) {
                    this.unitUI.EnableMovePathToTarget(target.position);
                } else {

                    if(this._currentPoint.HasValue) {
                        this._previousPoint = this._currentPoint;
                        this.debugPreviousPOint = this._currentPoint.Value;
                    }

                    this._currentPoint = null;
                    this.debugCurrentPoint = Vector2.zero;

                    this.unitUI.DisableMovePath();
                }
            }

            return true;
        }

        public override void InitiateTarget() {
            
            if(this.IsAlly(this._currentTarget)) {

                if(!this.CanHeal)
                    return;

                this._nextState = UnitState.SPECIAL;

                if(this._currentPoint.HasValue && this.CanMove) {
                    this.InitiateMove();
                } else if(this.TargetInRange()) {
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

            if(this._nextState == UnitState.SPECIAL) {
                float distance = Vector3.Distance(this.position, this._currentTarget.position);

                if(distance > (this._healingRange + this._unitRadius))
                    return false;
                else
                    return true;

            } else {
                return base.TargetInRange();
            }
        }

        public override void ProjectileCollisionEvent() {
            if(this._currentState == UnitState.SPECIAL) {
                this.InternalHeal();
            } else {
                base.ProjectileCollisionEvent();
            }
        }

        protected override void CheckStandbyState(out bool value) {
            base.CheckStandbyState(out value);

            if(this._currentState == UnitState.HEAL_STANDBY) {
                if(this.IsEnemy(this._currentTarget)) {
                    this._currentTarget = null;
                    this._previousTarget = null;

                    value = false;
                    return;
                }

                bool targetInRange = false;

                float distance = Vector3.Distance(this.position, this._currentTarget.position);
                if(distance + this._unitRadius < this._healingRange)
                    targetInRange = true;

                if(!targetInRange) {
                    Collider[] hits = Physics.OverlapSphere(this.position, this._healingRange, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
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
        public void Heal() {
            Debug.Log("HEALING UNIT");

            Debug.Log("Current Target: " + this._currentTarget.gameObject.name);
            Debug.Log("Current Target Position: " + this._currentTarget.position.ToString());

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
        }
        #endregion

        ///////////////////
        //// ANIMATION ////
        ///////////////////
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
        #endregion
    }
}