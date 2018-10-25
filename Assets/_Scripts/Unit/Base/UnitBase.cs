﻿namespace Unit {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Utility;

    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public abstract class UnitBase : HasHealthBase, IUnit {

        #region VARIABLE
        ////////////////
        ///// UNIT /////
        ////////////////
        [Header("UNIT")]
        [SerializeField] protected UnitType _unitType = UnitType.NONE;
        [SerializeField] protected UnitState _unitState = UnitState.NONE;
        [SerializeField] protected float _unitRadius = 0.0f;

        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        [SerializeField] protected IHasHealth _currentTarget = null;
        [SerializeField] protected IHasHealth _previousTarget = null;

        protected RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);
        protected NavMeshAgent _navMeshAgent = null;

        public override EntityType entityType { get { return EntityType.UNIT; } }
        public LayerMask areaMask { get { return this._navMeshAgent.areaMask; } }

        public UnitType unitType { get { return this._unitType; } }
        public UnitState unitState { get { return this._unitState; } set { this._unitState = value; } }
        public float unitRadius { get { return this._unitRadius; } }

        public Vector3 currentPoint { get { return this._currentPoint.Value; } }
        public Vector3 previousPoint { get { return this._previousPoint.Value; } }
        public IHasHealth currentTarget { get { return this._currentTarget; } }
        public IHasHealth previousTarget { get { return this._previousTarget; } }

        //////////////////
        //// MOVEMENT ////
        //////////////////
        [Header("UNIT - MOVENENT")]
        [SerializeField] protected MovementType _movementType = MovementType.NONE;
        [SerializeField, Range(1.0f, 50.0f)] protected float _maxStamina = 10.0f;
        [SerializeField, ReadOnly] protected float _currentStamina = 0.0f;
        [SerializeField, Range(0.0001f, 1.0f)] protected float _minMoveThreashold = 0.01f;
        [SerializeField, Range(1.0f, 50.0f)] protected float _moveSpeed = 5.0f;
        [SerializeField, Range(0.1f, 10.0f)] private float _allowedMovementImprecision = 1.0f;

        [SerializeField, ReadOnly] protected float _initialMovementDistance = 0.0f;
        [SerializeField, ReadOnly] protected bool _canMove = true;

        private Vector3 _velocity = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private Vector3 _resetPosition = Vector3.zero;
        protected NavMeshPath _unitPathing;
        
        public MovementType movementType { get { return this.movementType; } }
        public float currentStamina { get { return this._currentStamina; } }
        public float moveSpeed { get { return this._moveSpeed; } }

        public bool canMove { get { return this._canMove; } }
        public bool isMoving { get { return this._velocity.sqrMagnitude > (this._minMoveThreashold * this._minMoveThreashold); } }
        public virtual bool isIdle { get { return !this.isMoving && !this.isDead; } }
        public Vector3 lastPosition { get { return this._lastPosition; } set { this._lastPosition = value; } }

        ////////////////
        //// ATTACK ////
        ////////////////
        [Header("UNIT - ATTACK")]
        [SerializeField] protected GameObject _projectile = null;
        [SerializeField] protected Transform _projectileReleasePoint = null;
        [SerializeField] protected float _projectileSpeed = 10.0f;

        [SerializeField, Range(0.0f, 100.0f)] protected float _minDamage = 20.0f;
        [SerializeField, Range(0.0f, 100.0f)] protected float _maxDamage = 20.0f;
        [SerializeField, Range(1.0f, 50.0f)] protected float _attackRadius = 7.5f;

        [SerializeField] protected AttackType _attackType = AttackType.NONE;
        //[SerializeField] protected ArmorType _armorType = ArmorType.NONE;
        [SerializeField] protected AttackType _resistanceType = AttackType.NONE;
        [SerializeField] protected AttackType _weaknessType = AttackType.NONE;
        [SerializeField, Range(0.0f, 3.0f)] protected float _resistanceMultiplier = 0.5f;
        [SerializeField, Range(0.0f, 3.0f)] protected float _weaknessMultiplier = 0.5f;

        [SerializeField, ReadOnly] protected bool _canAttack = true;
        protected float _lastAttack = 0.0f;

        public float minDamage { get { return this._minDamage; } }
        public float maxDamage { get { return this._maxDamage; } }
        public float attackRadius { get { return this._attackRadius; } }

        public AttackType attackType { get { return this._attackType; } }
        //public ArmorType armorType { get { return this._armorType; } }
        public AttackType resistanceType { get { return this._resistanceType; } }
        public AttackType weaknessType { get { return this._weaknessType; } }
        public float resistancePercentage { get { return this._resistanceMultiplier; } }
        public float weaknessPercentage { get { return this._weaknessMultiplier; } }

        public bool canAttack { get { return this._canAttack; } }

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        [Header("UNIT - ANIMATION")]
        [SerializeField] protected float _endOfAttackClipTime = 0.8f;
        protected Animator _unitAnimator = null;

        [SerializeField] protected GameObject _deathPrefab = null;
        [SerializeField, Range(1.0f, 50.0f)] protected float _explosionRadius = 10.0f;
        [SerializeField, Range(1.0f, 1000.0f)] protected float _explosionForce = 300.0f;

        [Header("UNIT - DEBUGGING")]
        [ReadOnly]
        public Vector3 debugCurrentPoint = Vector3.zero;
        [ReadOnly]
        public Vector3 debugPreviousPOint = Vector3.zero;

        [Header("UNIT - UI")]
        public LineRenderDrawCircle radiusDrawer = null;
        #endregion

        #region UNITY
        protected virtual void Awake() {
            this.radiusDrawer.TurnOff();

            this._unitAnimator = this.GetComponent<Animator>();
            if(this._unitAnimator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            //this.SetupAttackEventAnimation();

            if(this._unitType == UnitType.NONE)
                throw new ArgumentException("Unit Type Needs to be Set To Value Other than NONE or ANY");

            this._navMeshAgent = this.GetComponent<NavMeshAgent>();
            this._navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            var collider = this.GetComponent<CapsuleCollider>();
            this._unitRadius = collider != null ? collider.radius : this.GetComponent<SphereCollider>().radius;
        }

        protected override void OnEnable() {
            base.OnEnable();

            this._unitState = UnitState.IDLE;
            this._canAttack = true;
            this._canMove = true;

            this._currentStamina = this._maxStamina;

            this._lastPosition = this.transform.position;
            this._resetPosition = this.position;
            this._navMeshAgent.speed = this._moveSpeed;
        }

        protected override void OnDisable() {
            base.OnDisable();

            this._unitState = UnitState.NONE;
            this._canAttack = false;
            this._canMove = false;

            this._lastPosition = Vector3.zero;
            this._navMeshAgent.speed = 0;
        }

        protected virtual void Update() {
            if(!this.isDead && this._unitState != UnitState.DEAD && this._unitState != UnitState.NONE && this._unitState != UnitState.ANY)
                this.UpdateUnit();
        }

        // For Debugging Purpose.
        public void OnDrawGizmos() {
            if(this._unitState == UnitState.ATTACK_ANIMATION) {
                //Gizmos.color = Color.red;
                //Gizmos.DrawSphere(this.transform.position, this._attackRadius);
            }
        }

        #endregion

        #region CLASS
        ///////////////////
        //// UNITSTATE ////
        ///////////////////
        #region UNIT_STATE
        protected virtual void SPAWNSTATE() {

        }

        protected virtual void IDLESTATE() {

        }

        protected virtual void ATTACKSTANDBYSTATE() {

        }

        protected virtual void ATTACKANIMATIONSTATE() {

        }

        protected virtual void ATTACKSTATE() {

        }

        protected virtual void MOVINGSTANDBYSTATE() {

        }

        protected virtual void MOVINGSTATE() {
            this.CheckMovement();
        }

        protected virtual void FINISHEDSTATE() {

        }

        protected virtual void DEADSTATE() {

        }

        protected virtual void CheckStandbyState(out bool value) {
            if(this._unitState == UnitState.ATTACK_STANDBY) {
                if(this.IsAlly(this._currentTarget)) {
                    this._currentTarget = null;
                    this._previousTarget = null;
                    // NOTE: display ui messsage indicating that the target is an allay.
                    value = false;
                    Debug.Log("Ally Selected");
                    return;
                }

                Debug.Log("Ally Selected - Cannot Access");

                bool targetInRange = false;

                // Distance checks to see if the target is within range of the attack radius. Doesn't take into account differernt size bounds of geometry, just the center point position.
                float distance = Vector3.Distance(this.position, this._currentTarget.position);
                if(distance + this._unitRadius < this._attackRadius)
                    targetInRange = true;

                // Seoncdary check using unity OverlapSphere to hit any unit/structure colliders within the attack radius.
                if(!targetInRange) {
                    Collider[] hits = Physics.OverlapSphere(this.position, this._attackRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
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
                    // NOTEL display ui message stating the target is out of range.
                    value = targetInRange;
                    return;
                }
            }

            value = true;
        }
        #endregion

        //////////////
        //// UNIT ////
        //////////////
        #region UNIT
        public virtual bool SetPoint(Vector3 point) {
            Vector3 position = Vector3.zero;

            if(!Utils.SamplePosition(point, out position)) {
                return false;
            }

            if(this._currentPoint.HasValue) {
                this._previousPoint = this._currentPoint;
                this.debugPreviousPOint = this._previousPoint.Value;
            }

            this._currentPoint = position;
            this.debugCurrentPoint = position;

            if(this._previousPoint.HasValue && this._currentPoint.HasValue) {
                if(this._currentPoint.Value.Equals(this._previousPoint.Value))
                    return false;
            }

            if(this._unitState == UnitState.MOVING_STANDBY || this.unitState == UnitState.MOVING) {
                this.MoveTo(position);
            }

            return true;
        }

        public virtual bool SetTarget(IHasHealth target) {
            bool targetIsValid = false;

            if(this._currentTarget != null)
                this._previousTarget = this._currentTarget;

            this._currentTarget = target;

            this.CheckStandbyState(out targetIsValid);

            Debug.Log("Target Selected Is Value? " + targetIsValid.ToString());

            /*if(this._unitState == UnitState.ATTACK_STANDBY) {
                if(this.IsAlly(target)) {
                    this._currentTarget = null;
                    this._previousTarget = null;
                    // NOTE: display ui messsage indicating that the target is an allay.
                    return false;
                }

                bool targetInRange = false;

                // Distance checks to see if the target is within range of the attack radius. Doesn't take into account differernt size bounds of geometry, just the center point position.
                float distance = Vector3.Distance(this.position, target.position);
                if(distance + this._unitRadius > this._attackRadius)
                    targetInRange = false;
                else
                    targetInRange = true;

                // Seoncdary check using unity OverlapSphere to hit any unit/structure colliders within the attack radius.
                if(!targetInRange) {
                    Collider[] hits = Physics.OverlapSphere(this.position, this._attackRadius, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
                    for(int i = 0; i < hits.Length; i++) {
                        IHasHealth hitHasHealth = hits[i].GetEntity<IHasHealth>();

                        if(hitHasHealth == null)
                            continue;

                        if(hitHasHealth != target)
                            continue;
                        else {
                            targetInRange = true;
                            break;
                        }
                    }
                }

                if(targetInRange && this.IsEnemy(target)) {
                    this.StartAttackAnimation();
                } else {
                    // NOTEL display ui message stating the target is out of range.
                    return targetInRange;
                }
            }*/

            return targetIsValid;
        }

        public virtual void ProjectileCollisionEvent() {
            if(this._unitState == UnitState.ATTACK_ANIMATION) {
                this._unitState = UnitState.ATTACK;
                this.InternalAttack(this.GetDamage());
            }
        }

        protected virtual void UpdateUnit() {
            switch(this._unitState) {
                case UnitState.MOVING:
                this.MOVINGSTATE();
                break;

                case UnitState.ATTACK_STANDBY:
                this.ATTACKSTANDBYSTATE();
                break;
            }
        }
            
        public virtual void NewTurn() {
            this._unitState = UnitState.IDLE;
            this._canMove = true;
            this._canAttack = true;
            this._lastPosition = this.position;
            this._resetPosition = this.position;
        }

        public virtual void Finished() {
            this._unitState = UnitState.FINISH;
        }

        public virtual void UnitDeath() {
            this.controller.RemoveFromUnitCap(2); // Remove unit Cap Cost from player.

            if(this._deathPrefab != null)
                this.PlayDeathAnimation();
        }
        #endregion

        //////////////////
        //// MOVEMENT ////
        //////////////////
        #region MOVEMENT
        public virtual void MoveTo(Vector3 dest) {
            NavMeshPath path = new NavMeshPath();

            this.StopMoving();

            this._resetPosition = this.position;
            this._navMeshAgent.CalculatePath(dest, path);
            this._navMeshAgent.SetPath(path);
            this._unitPathing = path;

            if(this._unitPathing.status == NavMeshPathStatus.PathComplete) {
                this._initialMovementDistance = this._navMeshAgent.remainingDistance;

                if(float.IsInfinity(_initialMovementDistance)) {
                    Debug.LogWarning("Remaining Distance Set To Infinity Using Backup Method: Corners - " + this._navMeshAgent.path.corners.Length.ToString());
                    float finalDistance = 0;
                    Vector3[] corners = this._navMeshAgent.path.corners;

                    for(int i = 0; i < corners.Length-1; ++i)
                        finalDistance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);

                    this._initialMovementDistance = finalDistance;
                }
                    
                this._navMeshAgent.isStopped = false;
                this.lastPosition = this.position;
            } else {
                this._navMeshAgent.SetDestination(dest);
                this._initialMovementDistance = this._navMeshAgent.remainingDistance;
            }

            this._unitState = UnitState.MOVING;
        }

        public virtual void CancelMove() {
            if(this._lastPosition.Equals(this.position)) {
                this.StopMoving();
                return;
            }

            this._navMeshAgent.enabled = false;
            this.position = this._resetPosition;
            this._navMeshAgent.enabled = true;
            this.StopMoving();
            this._unitState = UnitState.IDLE;
        }

        public virtual void FinishMove() {
            this._canMove = false;
            this._unitState = UnitState.IDLE;
            this.StopMoving();
        }

        public virtual void StopMoving() {
            this._navMeshAgent.isStopped = true;
            this._navMeshAgent.ResetPath();
            this._lastPosition = this.position;
        }

        public void LookAt(Vector3 pos) {
            this.transform.LookAt(new Vector3(pos.x, this.transform.position.y, pos.z), Vector3.up);
        }

        private bool SamplePosition(Vector3 dest, out Vector3 position) {
            NavMeshHit hit;

            if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.areaMask)) {
                position = hit.position;
                return true;
            }else {
                position = Vector3.zero;
                return false;
            }
        }

        private void CheckMovement() {
            // Seocondary Code to calculate the remainging distance. incase the one on the bottom doesnt work.
            /*float distance = 0.0f;
            Vector3[] corners = this._navMeshAgent.path.corners;
            for(int c = 0; c < corners.Length - 1; ++c) {
                distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);

            }*/
            if(this.currentStamina <= 0.0f) {
                this._canMove = false;
                this._unitState = UnitState.IDLE;
                ((UI.UnitUI)this._uiComponent).FinishMove();
                this.StopMoving();
            }

            //Debug.Log("Remaining Distance: " + this._navMeshAgent.remainingDistance.ToString());
            if(this._navMeshAgent.remainingDistance <= this._navMeshAgent.stoppingDistance) {
                if(!this._navMeshAgent.hasPath || this._navMeshAgent.velocity.sqrMagnitude == 0.0f) {
                    this._unitState = UnitState.MOVING_STANDBY;
                }
            } else {

                if(this._navMeshAgent.remainingDistance == this._initialMovementDistance)
                    return;

                // Stamina Calculations go here.
            }
        }
        #endregion

        ////////////////
        //// ATTACK ////
        ////////////////
        #region ATTACK
        public virtual void Attack() {
            Vector3 releasePosition = this._projectileReleasePoint.position;

            if(this._projectile == null) {
                this._unitState = UnitState.ATTACK;
                this.InternalAttack(this.GetDamage());
            } else {
                GameObject temp = Instantiate(this._projectile);
                Projectile tempProjjectile = temp.GetComponent<Projectile>() as Projectile;

                if(tempProjjectile == null)
                    temp.AddComponent<Projectile>();
                else
                    tempProjjectile.SetupTarget(this as IHasHealth, this._currentTarget, releasePosition, this._projectileSpeed);
            }
        }

        public float GetDamage() {
            float damage = UnityEngine.Random.Range(this._minDamage, this._maxDamage);
            return Mathf.Round(damage);
        }

        public override bool ReceiveDamage(float damage, IHasHealth incoming) {
            if(this.isDead)
                return true;

            ICanAttack attacker = incoming as ICanAttack;
            float finalDamage = damage;

            // Unit Based Multipliers
            if(attacker.attackType == this.resistanceType)
                finalDamage = Mathf.Round(damage * this.resistancePercentage);
            else if(attacker.attackType == this.weaknessType)
                finalDamage = Mathf.Round(damage * this.weaknessPercentage);
            
            Debug.Log("Attack Calculation: " + finalDamage.ToString());

            // Height based Multipliers
            float heightCalulations = this.position.y - attacker.position.y;
            bool isWithinHeightThreshold = (Mathf.Abs(heightCalulations) >= UnitValues.ATTACKHEIGHTTHRESHOLD) ? true : false;
            bool isAttackerBelow = (Mathf.Sign(heightCalulations) == 1) ? true : false;

            Debug.Log("Height Threshold: " + Mathf.Abs(heightCalulations));

            if(isWithinHeightThreshold) {
                if(isAttackerBelow)
                    finalDamage = Mathf.Round(finalDamage * UnitValues.ABOVEHEIGHTMULTIPLIER);
                else
                    finalDamage = Mathf.Round(finalDamage * UnitValues.BELOWHEIGHTMULTIPLIER);

                Debug.Log("Height Calculations: " + finalDamage.ToString());
            }

            // Check to never go beyond negative values.
            if(finalDamage <= 0.0f)
                finalDamage = 0.0f;

            Debug.Log("Current Target " + this.name + ": Took " + finalDamage.ToString() + " of Damage from - " + incoming.gameObject.name);
            this.RemoveHealth(finalDamage);
            this.uiComponent.UpdateUI();

            if(this.currentHealth <= 0.0f) {
                if(this.controller != null)
                    this.controller.units.Remove(this);

                // NOTE: play any death animations, or add in any death effects onto the scene. 
                this.UnitDeath();

                UnitPoolManager.instance.Return(this);

                return true;
            } else {
                // NOTE: play hit anbimation.
            }
            return false;
        }

        public override bool ReceiveDamage(float damage, IHasHealth target, Vector3 origin) {
            if(this.isDead)
                return true;

            this.PlayHitAnimations(origin);

            return this.ReceiveDamage(damage, target);
        }

        protected virtual void InternalAttack(float damage) {
            this.SpawnAttackParticle();

            this._currentTarget.lastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth, this.transform.position);

            ((UI.UnitUI)this._uiComponent).FinishAttack();
            this._unitState = UnitState.IDLE;
            this._canAttack = false;
        }

        protected virtual void SpawnAttackParticle() {
            if(this._projectile != null) {
                Vector3 relativePos = this.position - this._currentTarget.position;
                Quaternion tempRotation = Quaternion.LookRotation(relativePos);

                ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_RANGE, this._currentTarget.position, tempRotation);
            } else {
                ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_MELEE, this._currentTarget.position);
            }
        }
        #endregion

        ///////////////////
        //// ANIMATION ////
        ///////////////////
        #region ANIMATION
        public virtual void SetupAnimation() {
            this._unitAnimator = this.GetComponent<Animator>();
            if(this._unitAnimator == null)
                throw new ArgumentNullException("Unit Animator Is Missing");

            this.SetupEventAnimation();
        }

        public virtual void PlayHitAnimations(Vector3 origin) {
            Vector3 newOrigin = new Vector3(origin.x, 0.0f, origin.z);
            Vector3 newPos = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);

            Vector3 directionNormal = (newOrigin - newPos).normalized;
            float signedAngle = Vector3.SignedAngle(directionNormal, this.transform.forward, Vector3.up);
            float angleInDegrees = (signedAngle + 360.0f) % 360;

            Debug.Log("Angle TO Degrees: " + angleInDegrees.ToString());

            if(angleInDegrees <= UnitValues.FRONT_L || angleInDegrees >= UnitValues.FRONT_R) {
                // FRONt.
                this._unitAnimator.Play("HitFromFront");
                Debug.Log("ATTACKED FRON FRONT");
            } else if(angleInDegrees >= UnitValues.BEHIND_L && angleInDegrees <= UnitValues.BEHIND_R) {
                // BEHIND
                this._unitAnimator.Play("HitFromBehind");
                Debug.Log("ATTACKED FROM BEHIND");
            } else if(angleInDegrees > UnitValues.FRONT_L && angleInDegrees < UnitValues.BEHIND_L) {
                // LEFT
                this._unitAnimator.Play("HitFromLeft");
                Debug.Log("ATTACKED FROM LEFT");
            } else if(angleInDegrees > UnitValues.BEHIND_R && angleInDegrees < UnitValues.FRONT_R) {
                // RIGHT
                this._unitAnimator.Play("HitFromRight");
                Debug.Log("ATTACKED FROM RIGHT");
            } else {
                Debug.LogError("Angle TO Degree Is Out Of Bounds, Produced Result: " + angleInDegrees.ToString());
            }
        }

        public virtual void StartStateAnimation() {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(this._currentTarget.position);
            this.StopMoving();

            if(this._unitState == UnitState.ATTACK_STANDBY) {
                this._unitState = UnitState.ATTACK_ANIMATION;
                this._unitAnimator.Play("Attack");
            }
        }

        protected virtual void PlayDeathAnimation() {
            Transform uTransform = this.transform;
            Vector3 uPoseition = this.position;
            Quaternion uRotation = this.rotation;

            // Move the unit below the stage.
            this.transform.position = new Vector3(uPoseition.x, -100.0f, uPoseition.z);

            // Calculate the position and force for the Physics Force.
            Vector3 explsioonDirection = (this.lastAttacker.position - uPoseition).normalized;
            Vector3 explosionPosition = uPoseition + explsioonDirection * this._unitRadius;

            // Zero out the directional Normal y to even out the stage.
            explsioonDirection.y = 0.0f;

            // Create the death prefab and grab its rigidbody
            GameObject deathGO = Instantiate(this._deathPrefab, uPoseition, uRotation);
            deathGO.ColorRenderers(this.controller.color);
            Rigidbody[] deathRB = deathGO.GetComponentsInChildren<Rigidbody>() as Rigidbody[];

            foreach(Rigidbody rb in deathRB) {
                rb.AddForceAtPosition(explsioonDirection * this._explosionForce, explosionPosition);
            }

            UnitPoolManager.instance.AddUnitDeath(deathGO, UnitValues.DEATHCOUNTER);

        }

        protected virtual void SetupEventAnimation() {
            AnimationEvent animEvent = new AnimationEvent();
            AnimationClip animcLip = new AnimationClip();

            Debug.Log("SETUP ATTACK ANIMATION EVENT");

            /*if(this._endOfAttackClipTime <= 0.0f)
                throw new ArgumentException("End of Attack Animation Timer Needs to be Set, Cannot Be Set At 0 Seconds");*/

            animEvent.time = this._endOfAttackClipTime;
            animEvent.functionName = "Attack";

            foreach(AnimationClip clip in this._unitAnimator.runtimeAnimatorController.animationClips) {
                if(clip.name.Contains("Attack")) {
                    // Function To Help Find Frame Event Time.
                    /*foreach(AnimationEvent evt in clip.events) {
                        Debug.Log("Event Attack Time: " + evt.time);
                        UnityEditor.EditorApplication.isPaused = true;
                    }*/
                    animcLip = clip;
                    break;
                }
            }

            animcLip.AddEvent(animEvent);
        }
        #endregion
        #endregion
    }
}