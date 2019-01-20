﻿namespace Unit {

    using UnityEngine;
    using UnityEngine.AI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Scriptable;
    using Utility;
    using Player;

    [System.Serializable, RequireComponent(typeof(NavMeshAgent))]
    public abstract class UnitBase : HasHealthBase, IUnit {

        #region VARIABLE

        [Header("UNIT")]
        [SerializeField] protected UnitScriptable _data;

        [SerializeField] protected UnitClassType _classType = UnitClassType.NONE;
        [SerializeField] protected UnitType _unitType = UnitType.NONE;
        [SerializeField] protected UnitState _unitState = UnitState.NONE;
        [SerializeField] protected MovementType _moveType = MovementType.NONE;
        [SerializeField] protected AttackType _attackType = AttackType.NONE;
        //[SerializeField] protected ArmorType _armorType = ArmorType.NONE;
        [SerializeField] protected AttackType _resistanceType = AttackType.NONE;
        [SerializeField] protected AttackType _weaknessType = AttackType.NONE;

        [SerializeField, HideInInspector] protected float _unitRadius = 0.0f;

        protected RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);
        [SerializeField] protected NavMeshAgent _navMeshAgent = null;

        [Header("UNIT - TARGET")]
        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        [SerializeField] protected IHasHealth _currentTarget = null;
        [SerializeField] protected IHasHealth _previousTarget = null;

        [Header("UNIT - MOVENENT")]
        protected float _minMoveThreashold = 0.01f;
        [SerializeField] protected float _moveSpeed = 0.0f;
        private float _allowedMovementImprecision = 1.0f;

        [SerializeField, ReadOnly] protected float _initialMovementDistance = 0.0f;
        [SerializeField, ReadOnly] protected bool _canMove = true;

        protected Vector3 _velocity = Vector3.zero;
        protected Vector3 _lastPosition = Vector3.zero;
        protected Vector3 _resetPosition = Vector3.zero;
        protected NavMeshPath _unitPathing;

        [Header("UNIT - ATTACK")]
        [SerializeField] protected float _projectileSpeed = 0.0f;
        [SerializeField] protected float _attackRange = 0.0f;
        [SerializeField] protected float _minDamage = 0.0f;
        [SerializeField] protected float _maxDamage = 0.0f;
        protected float _lastAttack = 0.0f;
        [SerializeField] protected float _resistanceMultiplier = 0.0f;
        [SerializeField] protected float _weaknessMultiplier = 0.0f;

        [SerializeField] protected bool _canAttack = true;

        [SerializeField] protected Transform _projectileReleasePoint = null;

        [SerializeField] protected GameObject _projectilePrefan = null;

        [Header("UNIT - ANIMATION")]
        [SerializeField] protected float _endOfAttackClipTime = 0.8f;
        [SerializeField] protected float _explosionRadius = 0.0f;
        [SerializeField] protected float _explosionForce = 0.0f;

        [SerializeField] protected Animator _unitAnimator = null;

        [SerializeField] protected GameObject _deathPrefab = null;

        [Header("UNIT - DEBUGGING")]
        public Vector3 debugCurrentPoint = Vector3.zero;
        public Vector3 debugPreviousPOint = Vector3.zero;

        [Header("UNIT - UI")]
        public LineRenderDrawCircle radiusDrawer = null;

        public UI.UnitUI unitUI { get { return this.uiBase as UI.UnitUI; } }

        public UnitScriptable Data { get { return this._data; } }

        public override EntityType entityType { get { return EntityType.UNIT; } }
        public UnitClassType classType { get { return this._classType; } }
        public UnitType unitType { get { return this._unitType; } }
        public UnitState unitState { get { return this._unitState; } set { this._unitState = value; } }
        public MovementType moveType { get { return this.moveType; } }
        public AttackType attackType { get { return this._attackType; } }
        //public ArmorType armorType { get { return this._armorType; } }
        public AttackType resistanceType { get { return this._resistanceType; } }
        public AttackType weaknessType { get { return this._weaknessType; } }

        public float UnitRadius { get { return this._unitRadius; } }

        public LayerMask AreaMask { get { return this._navMeshAgent.areaMask; } }

        public Vector3 CurrentPoint { get { return this._currentPoint.Value; } }
        public Vector3 PreviousPoint { get { return this._previousPoint.Value; } }
        public IHasHealth CurrentTarget { get { return this._currentTarget; } }
        public IHasHealth PreviousTarget { get { return this._previousTarget; } }

        public float MoveSpeed { get { return this._moveSpeed; } }
        public bool CanMove { get { return this._canMove; } }
        public bool IsMoving { get { return this._velocity.sqrMagnitude > (this._minMoveThreashold * this._minMoveThreashold); } }
        public virtual bool IsIdle { get { return !this.IsMoving && !this.isDead; } }
        public Vector3 LastPosition { get { return this._lastPosition; } set { this._lastPosition = value; } }

        public float AttackRange { get { return this._attackRange; } }
        public float MinDamage { get { return this._minDamage; } }
        public float MaxDamage { get { return this._maxDamage; } }
        public float ResistancePercentage { get { return this._resistanceMultiplier; } }
        public float WeaknessPercentage { get { return this._weaknessMultiplier; } }

        public bool CanAttack { get { return this._canAttack; } }

        #endregion

        #region CLASS_CLEAN

        public bool SetData(UnitScriptable data) {
            if(data == null)
                return false;

            this._data = data;

            return true;
        }

        public override void Setup() {

            this._projectilePrefan = this._data.projectilePrefab;
            this._deathPrefab = this._data.deathPrefab;

            this._unitState = UnitState.NONE;

            this._classType = this._data.classType;
            this._unitType = this._data.unitType;
            this._attackType = this._data.attackType;
            this._resistanceType = this._data.resistanceType;
            this._weaknessType = this._data.weaknessType;
            this._moveType = this._data.moveType;

            this._currentHealth = this._data.health;
            this._maxHealth = this._data.health;
            this._currentEnergy = this._data.stamina;
            this._maxEnergy = this._data.stamina;

            this._resistanceMultiplier = this._data.resistanceMultiplier;
            this._weaknessMultiplier = this._data.weaknessMultiplier;
            this._minDamage = this._data.minDamage;
            this._maxDamage = this._data.maxDamage;
            this._attackRange = this._data.attackRange;

            this._moveSpeed = this._data.moveSpeed;

            this._projectileSpeed = this._data.projectileSpeed;
            this._endOfAttackClipTime = this._data.endOfAttackClipTime;
            this._explosionRadius = this._data.explosionRadius;
            this._explosionForce = this._data.explosionForce;

            this._unitAnimator = this.GetComponent<Animator>();
            if(this._unitAnimator == null) {
                Debug.LogError("The Unit (" + this.gameObject.name + ") Doesn't Have An Animator Component");
                throw new System.ArgumentNullException("Unit Animator Is Missing");
            }

            this._navMeshAgent = this.GetComponent<NavMeshAgent>();
            if(this._navMeshAgent == null) {
                Debug.LogWarning("Unit Doesn't Contain A NavMeshAgent Component, Add One Now!");
                this._navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
            }
            this._navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            CapsuleCollider collider = this.GetComponent<CapsuleCollider>() as CapsuleCollider;
            this._unitRadius = collider != null ? collider.radius : this.GetComponent<SphereCollider>().radius;

            if(this._unitType == UnitType.NONE) {
                Debug.Log("Unit Type Can Not be Type of NONE");
                throw new System.ArgumentException("Unit Type Needs to be Set To Value Other than NONE or ANY");
            }

            if(this.radiusDrawer == null) {
                this.radiusDrawer = this.transform.Find("RadiusDrawer").GetComponent<LineRenderDrawCircle>() as LineRenderDrawCircle;
                this.radiusDrawer.TurnOff();
            } else
                this.radiusDrawer.TurnOff();

            base.Setup();

            this.unitUI.Setup(this);
        }

        public override void Init(Player contoller) {
            base.Init(contoller);

            this._unitState = UnitState.IDLE;
            this._canAttack = true;
            this._canMove = true;

            this._currentHealth = this._data.health;
            this._maxHealth = this._data.health;
            this._currentEnergy = this._data.stamina;
            this._maxEnergy = this._data.stamina;

            this._resistanceMultiplier = this._data.resistanceMultiplier;
            this._weaknessMultiplier = this._data.weaknessMultiplier;
            this._minDamage = this._data.minDamage;
            this._maxDamage = this._data.maxDamage;
            this._attackRange = this._data.attackRange;

            this._moveSpeed = this._data.moveSpeed;

            this._lastPosition = this.transform.position;
            this._resetPosition = this.transform.position;
            this._navMeshAgent.speed = this._moveSpeed;
        }

        public override void Return() {
            this._unitState = UnitState.NONE;
            this._canAttack = false;
            this._canMove = false;

            this._currentHealth = this._data.health;
            this._maxHealth = this._data.health;
            this._currentEnergy = this._data.stamina;
            this._maxEnergy = this._data.stamina;

            this._resistanceMultiplier = this._data.resistanceMultiplier;
            this._weaknessMultiplier = this._data.weaknessMultiplier;
            this._minDamage = this._data.minDamage;
            this._maxDamage = this._data.maxDamage;
            this._attackRange = this._data.attackRange;

            this._moveSpeed = this._data.moveSpeed;

            this._currentPoint = null;
            this._currentTarget = null;

            this._lastPosition = Vector3.zero;
            this._navMeshAgent.speed = 0;

            base.Return();
        }


        public virtual void ApplyUpgrade(UnitUpgradeType type, float value) {
            switch(type) {
                case UnitUpgradeType.ATTACK:
                this._minDamage += value;
                this._maxDamage += value;
                break;

                case UnitUpgradeType.HEALTH:
                this._currentHealth += value;
                this._maxHealth += value;
                break;

                case UnitUpgradeType.STAMINA:
                this._currentEnergy += value;
                this._maxEnergy += value;
                break;
            }
        }

        #endregion

        #region UNITY

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

        #region CLASS_CLEAN




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
                if(distance + this._unitRadius < this._attackRange)
                    targetInRange = true;

                // Seoncdary check using unity OverlapSphere to hit any unit/structure colliders within the attack radius.
                if(!targetInRange) {
                    Collider[] hits = Physics.OverlapSphere(this.position, this._attackRange, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
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

            ResourceManager.instance.RemoveResource(this.controller, PlayerResource.POPULATION, this._data.populationCost);

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
                this.LastPosition = this.position;
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

            if(NavMesh.SamplePosition(dest, out hit, this._allowedMovementImprecision, this.AreaMask)) {
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
            if(this._currentEnergy <= 0.0f) {
                this._canMove = false;
                this._unitState = UnitState.IDLE;
                ((UI.UnitUI)this.uiBase).FinishMove();
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

            if(this._projectilePrefan == null) {
                this._unitState = UnitState.ATTACK;
                this.InternalAttack(this.GetDamage());
            } else {
                GameObject temp = Instantiate(this._projectilePrefan);
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
                finalDamage = Mathf.Round(damage * this.ResistancePercentage);
            else if(attacker.attackType == this.weaknessType)
                finalDamage = Mathf.Round(damage * this.WeaknessPercentage);
            
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
            this.uiBase.UpdateUI();

            if(this.CurrentHealth <= 0.0f) {
                if(this.controller != null)
                    this.controller.RemoveUnit(this);

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

            this._currentTarget.LastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth, this.transform.position);

            ((UI.UnitUI)this.uiBase).FinishAttack();
            this._unitState = UnitState.IDLE;
            this._canAttack = false;
        }

        protected virtual void SpawnAttackParticle() {
            if(this._projectilePrefan != null) {
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
                throw new System.ArgumentNullException("Unit Animator Is Missing");

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
            Vector3 explsioonDirection = (this.LastAttacker.position - uPoseition).normalized;
            Vector3 explosionPosition = uPoseition + explsioonDirection * this._unitRadius;

            // Zero out the directional Normal y to even out the stage.
            explsioonDirection.y = 0.0f;

            // Create the death prefab and grab its rigidbody
            GameObject deathGO = Instantiate(this._deathPrefab, uPoseition, uRotation);
            deathGO.ColorRenderers(this.controller.PlayerColor);
            Rigidbody[] deathRB = deathGO.GetComponentsInChildren<Rigidbody>() as Rigidbody[];

            foreach(Rigidbody rb in deathRB) {
                rb.AddForceAtPosition(explsioonDirection * this._explosionForce, explosionPosition);
            }

            UnitPoolManager.instance.AddUnitDeath(deathGO, UnitValues.DEATHCOUNTER);

        }

        protected virtual void SetupEventAnimation() {
            AnimationEvent animEvent = new AnimationEvent();
            AnimationClip animcLip = new AnimationClip();

            /*if(this._endOfAttackClipTime <= 0.0f)
                throw new ArgumentException("End of Attack Animation Timer Needs to be Set, Cannot Be Set At 0 Seconds");*/

            animEvent.time = this._endOfAttackClipTime;
            animEvent.functionName = "Attack";

            foreach(AnimationClip clip in this._unitAnimator.runtimeAnimatorController.animationClips) {
                if(clip.name.Contains("Attack")) {
                    // Function To Help Find Frame Event Time.
                    /*foreach(AnimationEvent evt in clip.events) {
                        Debug.Log("Class Type: " + this._unitType.ToString());
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