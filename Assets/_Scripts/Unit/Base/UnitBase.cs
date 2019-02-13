namespace Unit {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using Scriptable;
    using Utility;
    using Player;

    [RequireComponent(typeof(UnitSound))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class UnitBase : HasHealthBase, IUnit {

        #region VARIABLE
        [SerializeField, HideInInspector] protected UnitScriptable _data;
        [SerializeField] protected UnitSound _unitSound = null;

        [Header("UNIT")]
        [SerializeField] protected UnitClassType _classType = UnitClassType.NONE;
        [SerializeField] protected UnitType _unitType = UnitType.NONE;

        [Space]
        [SerializeField] protected UnitState _previousState = UnitState.NONE; 
        [SerializeField] protected UnitState _currentState = UnitState.NONE;
        [SerializeField] protected UnitState _nextState = UnitState.NONE;

        [Space]
        [SerializeField] protected MovementType _moveType = MovementType.NONE;
        [SerializeField] protected AttackType _attackType = AttackType.NONE;
        [SerializeField] protected AttackType _resistanceType = AttackType.NONE;
        [SerializeField] protected AttackType _weaknessType = AttackType.NONE;

        [SerializeField, HideInInspector] protected float _unitRadius = 0.0f;

        protected RaycastHitDistanceSortComparer _hitComparer = new RaycastHitDistanceSortComparer(true);
        [SerializeField, HideInInspector] protected NavMeshAgent _navMeshAgent = null;

        protected Vector3? _currentPoint = null;
        protected Vector3? _previousPoint = null;
        protected IHasHealth _currentTarget = null;
        protected IHasHealth _previousTarget = null;

        [Header("UNIT - MOVENENT")]
        [SerializeField] protected bool _hasStamina = true;
        [SerializeField] protected bool _isMoving = false;
        [SerializeField] protected float _staminaToUse = 0.0f;
        protected float _minMoveThreashold = 0.01f;
        [SerializeField] protected float _moveSpeed = 0.0f;
        private float _allowedMoveImprecision = 1.0f;

        [SerializeField, ReadOnly] protected float _initialMoveDist = 0.0f;

        protected Vector3 _velocity = Vector3.zero;
        protected Vector3 _lastPosition = Vector3.zero;
        protected NavMeshPath _unitPathing;

        [Header("UNIT - ATTACK")]
        [SerializeField] protected bool _canAttack = true;

        [SerializeField] protected float _projectileSpeed = 0.0f;
        [SerializeField] protected float _attackRange = 0.0f;
        [SerializeField] protected float _minDamage = 0.0f;
        [SerializeField] protected float _maxDamage = 0.0f;
        protected float _lastAttack = 0.0f;
        [SerializeField] protected float _resistanceMultiplier = 0.0f;
        [SerializeField] protected float _weaknessMultiplier = 0.0f; 

        [Space]
        [SerializeField] protected Transform _projectileReleasePoint = null;

        [SerializeField] protected GameObject _projectilePrefab = null;

        [Header("UNIT - ANIMATION")]
        [SerializeField] protected float _endOfAttackClipTime = 0.8f;
        [SerializeField] protected float _explosionRadius = 0.0f;
        [SerializeField] protected float _explosionForce = 0.0f;

        [Space]
        [SerializeField, HideInInspector] protected Animator _unitAnimator = null;
        [SerializeField, HideInInspector] protected Animator _pedestalAnimator = null;

        [SerializeField] protected GameObject _deathPrefab = null;

        [Header("UNIT - DEBUGGING")]
        public Vector3 debugPreviousPOint = Vector3.zero;
        public Vector3 debugCurrentPoint = Vector3.zero;
        public HasHealthBase debugPreviousTarget = null;
        public HasHealthBase debugCurrentTarget = null;

        public UI.UnitUI unitUI { get { return this.uiBase as UI.UnitUI; } }

        public UnitScriptable Data { get { return this._data; } }

        public override EntityType entityType { get { return EntityType.UNIT; } }
        public UnitClassType classType { get { return this._classType; } }
        public UnitType unitType { get { return this._unitType; } }
        public UnitState PreviousState { get { return this._previousState; } }
        public UnitState CurrentState { get { return this._currentState; } set { this._currentState = value; } }
        public UnitState NextState { get { return this._nextState; } set { this._nextState = value; } }
        public MovementType moveType { get { return this.moveType; } }
        public AttackType attackType { get { return this._attackType; } }
        public AttackType resistanceType { get { return this._resistanceType; } }
        public AttackType weaknessType { get { return this._weaknessType; } }

        public float UnitRadius { get { return this._unitRadius; } }

        public LayerMask AreaMask { get { return this._navMeshAgent.areaMask; } }

        public Vector3 CurrentPoint { get { return this._currentPoint.Value; } }
        public Vector3 PreviousPoint { get { return this._previousPoint.Value; } }
        public IHasHealth CurrentTarget { get { return this._currentTarget; } }
        public IHasHealth PreviousTarget { get { return this._previousTarget; } }

        public float MoveSpeed { get { return this._moveSpeed; } }
        public bool CanMove { get { return (this._currentEnergy > 0.0f && !this._isMoving && !this._navMeshAgent.pathPending) ? true : false; } }
        public bool IsMoving { get { return this._isMoving; } }
        public virtual bool IsIdle { get { return !this.IsMoving && !this.IsDead; } }
        public Vector3 LastPosition { get { return this._lastPosition; } set { this._lastPosition = value; } }

        public float AttackRange { get { return this._attackRange; } }
        public float MinDamage { get { return this._minDamage; } }
        public float MaxDamage { get { return this._maxDamage; } }
        public float ResistancePercentage { get { return this._resistanceMultiplier; } }
        public float WeaknessPercentage { get { return this._weaknessMultiplier; } }

        public bool CanAttack { get { return this._canAttack; } }

        #endregion

        #region UNITY
        private void Update() {
            this.UpdateUnit();
        }

        #endregion

        #region CLASS
        public bool SetData(UnitScriptable data) {
            if(data == null)
                return false;

            this._data = data;

            return true;
        }

        public override void Setup() {

            this._projectilePrefab = this._data.projectilePrefab;
            this._deathPrefab = this._data.deathPrefab;

            this._currentState = UnitState.NONE;

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

            if(this._unitSound == null) {
                if(this.GetComponent<UnitSound>() == null)
                    this.gameObject.AddComponent<UnitSound>();
                else
                    this._unitSound = this.GetComponent<UnitSound>();
            }
            this._unitSound.Setup(this);

            this._unitAnimator = this.GetComponent<Animator>();
            if(this._unitAnimator == null) {
                Debug.LogError("The Unit (" + this.gameObject.name + ") Doesn't Have An Animator Component");
                throw new System.ArgumentNullException("Unit Animator Is Missing");
            }

            GameObject pedestal = this.transform.Find("pedestal").gameObject;
            if(pedestal == null)
                Debug.LogError("No Pedestal Attached To " + this.gameObject.name + " - Please Add One!");
            else {
                this._pedestalAnimator = pedestal.GetComponent<Animator>();
                if(this._pedestalAnimator == null) {
                    Debug.LogError("The Unit (" + this.gameObject.name + ") Doesn't Have An Animator Component On the Pedestal");
                    throw new System.ArgumentNullException("Unit Pedestal Animator Is Missing");
                }
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

            base.Setup();

            this.unitUI.Setup(this);
        }

        public override void Init(Player contoller) {
            base.Init(contoller);

            this.uiBase.Init(contoller);

            this._currentState = UnitState.IDLE;
            this._canAttack = true;

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
            this._navMeshAgent.speed = this._moveSpeed;
        }

        public override void Return() {
            this.uiBase.Return();

            this._currentState = UnitState.NONE;
            this._canAttack = false;

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

        public virtual void NewTurn() {
            this._currentState = UnitState.IDLE;
            this._canAttack = true;
            this._lastPosition = this.position;

            this._currentPoint = null;
            this._currentTarget = null;

            this._lastAttackers.Clear();
        }

        public virtual void Death() {

            ResourceManager.instance.RemoveResource(this.Controller, PlayerResource.POPULATION, this._data.populationCost);
            GoldMineManager.instance.RemoveEntity(this);

            if(this._deathPrefab != null)
                this.PlayDeathAnimation();
        }

        public virtual void Finished() {
            this._currentState = UnitState.FINISH;
        }

        public virtual bool SetPoint(Vector3 point) {

            if(!this.CanMove)
                return false;

            Vector3 position = Vector3.zero;
            if(!Utils.SamplePosition(point, out position))
                return false;

            if(this._currentPoint.HasValue) {
                this._previousPoint = this._currentPoint;
                this.debugPreviousPOint = this._previousPoint.Value;
            }

            this._currentPoint = position;
            this.debugCurrentPoint = position;

            this._currentTarget = null;
            this.debugCurrentTarget = null;

            this.unitUI.EnableMovePath(this._currentPoint.Value);

            return true;
        }

        public virtual bool SetTarget(IHasHealth target) {

            if(this.IsAlly(target))
                return false;

            if(!this._canAttack)
                return false;

            this.unitUI.EnableAttackRadius();

            if(this._currentTarget != null) {
                this._previousTarget = this._currentTarget;
                this.debugPreviousTarget = this._currentTarget as HasHealthBase;
            }
            this._currentTarget = target;
            this.debugCurrentTarget = target as HasHealthBase;

            if(!this.CanMove && !this.TargetInRange())
                return false;

            Vector3 point = Vector3.zero;
            if(this._currentTarget.entityType == EntityType.STRUCTURE) {
                Structure.StructureBase structure = target as Structure.StructureBase;
                point = Utils.ClosesPointToBounds(structure.ColliderBounds, this.position);

                //GameObject temp = new GameObject("Close Point");
                //temp.transform.position = point;
            } else {
                point = this._currentTarget.position;
            }

            if(!this.TargetInRange()) {
                if(this.CanMove)
                    this.unitUI.EnableMovePathToTarget(point, this._attackRange);
                else
                    return false;

            } else {

                if(this._currentPoint.HasValue) {
                    this._previousPoint = this._currentPoint;
                    this.debugPreviousPOint = this._previousPoint.Value;
                }

                this._currentPoint = null;
                this.debugCurrentPoint = Vector3.zero;

                this.unitUI.DisableMovePath();
            }

            return true;
        }

        public virtual void InitiateMove() {

            this.unitUI.MoveRadiusToOrigin();

            if(!this.CanMove)
                return;

            if(this._unitPathing.status == NavMeshPathStatus.PathInvalid || this._unitPathing.status == NavMeshPathStatus.PathPartial)
                return;

            if(!this._hasStamina)
                this._currentEnergy = 0.0f;
            else
                this._currentEnergy = this._staminaToUse;

            this.StopMoving();

            NavMeshPath path = new NavMeshPath();

            this._navMeshAgent.CalculatePath(this._currentPoint.Value, path);
            this._navMeshAgent.SetPath(path);
            this._unitPathing = path;

            this._navMeshAgent.SetPath(this._unitPathing);

            if(this._unitPathing.status == NavMeshPathStatus.PathComplete) {
                this._initialMoveDist = this._navMeshAgent.remainingDistance;

                if(float.IsInfinity(_initialMoveDist)) {
                    Debug.LogWarning("Remaining Distance Set To Infinity Using Backup Method: Corners - " + this._navMeshAgent.path.corners.Length.ToString());
                    float finalDistance = 0;
                    Vector3[] corners = this._navMeshAgent.path.corners;

                    for(int i = 0; i < corners.Length - 1; ++i)
                        finalDistance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);

                    this._initialMoveDist = finalDistance;
                }

                this._navMeshAgent.isStopped = false;

            } else {
                this._navMeshAgent.SetDestination(this._currentPoint.Value);
                this._initialMoveDist = this._navMeshAgent.remainingDistance;
            }

            this._isMoving = true;
            this._currentState = UnitState.MOVING;
        }

        public virtual void InitiateTarget() {

            this._nextState = UnitState.ATTACK;

            this.unitUI.MoveRadiusToOrigin();

            if(this._currentPoint.HasValue && this.CanMove) { // Move To Point.
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
                Debug.LogWarning("Can't Move To Target & Is Out Of Attack Range");
            }
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

        protected virtual void UpdateUnit() {
            if(this._currentState == UnitState.MOVING)
                this.CheckMovement();
        }
        #endregion

        #region MOVE
        // REMOVE
        public virtual void MoveTo(Vector3 dest) {
            NavMeshPath path = new NavMeshPath();

            this.StopMoving();

            this._navMeshAgent.CalculatePath(dest, path);
            this._navMeshAgent.SetPath(path);
            this._unitPathing = path;

            if(this._unitPathing.status == NavMeshPathStatus.PathComplete) {
                this._initialMoveDist = this._navMeshAgent.remainingDistance;

                if(float.IsInfinity(_initialMoveDist)) {
                    Debug.LogWarning("Remaining Distance Set To Infinity Using Backup Method: Corners - " + this._navMeshAgent.path.corners.Length.ToString());
                    float finalDistance = 0;
                    Vector3[] corners = this._navMeshAgent.path.corners;

                    for(int i = 0; i < corners.Length - 1; ++i)
                        finalDistance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);

                    this._initialMoveDist = finalDistance;
                }

                this._navMeshAgent.isStopped = false;
                this.LastPosition = this.position;
            } else {
                this._navMeshAgent.SetDestination(dest);
                this._initialMoveDist = this._navMeshAgent.remainingDistance;
            }

            this._currentState = UnitState.MOVING;
        }

        public virtual void StopMoving() {
            this.unitUI.DisableMovePath();

            this._isMoving = false;
            this._navMeshAgent.isStopped = true;
            this._navMeshAgent.ResetPath();
            this._lastPosition = this.position;
        }

        public void LookAt(Vector3 pos) {
            this.transform.LookAt(new Vector3(pos.x, this.transform.position.y, pos.z), Vector3.up);
        }

        public Vector3[] ReturnPathToPoint(Vector3 point) {

            Vector3[] temp;
            NavMeshPath path = new NavMeshPath();

            if(this._navMeshAgent.CalculatePath(point, path)) {
                this._unitPathing = path;
                temp = FinalizePath(false);
                return temp;
            } else {
                this._unitPathing.ClearCorners();
                return null;
            }
        }

        public Vector3[] ReturnPathToTarget(Vector3 point, float attackRange = 0.0f) {

            Vector3[] temp;
            NavMeshPath path = new NavMeshPath();
                
            if(this._navMeshAgent.CalculatePath(point, path)) {
                this._unitPathing = path;
                temp = FinalizePath(true, attackRange);
                return temp;
            } else {
                this._unitPathing.ClearCorners();
                return null;
            }
        }

        private Vector3[] FinalizePath(bool target, float attackRange = 0.0f) {
            Stack<Vector3> linePoints = new Stack<Vector3>();
            float castLerpSize = 0.5f;

            this._staminaToUse = this._currentEnergy;
            this._hasStamina = true;

            linePoints.Push(this._unitPathing.corners[0]);

            if(target) {
                Vector3 direction = (this._unitPathing.corners[this._unitPathing.corners.Length - 2] - this._unitPathing.corners[this._unitPathing.corners.Length - 1]).normalized;
                Vector3 attackPoint = this._unitPathing.corners[this._unitPathing.corners.Length - 1] + (direction * (attackRange + this._unitRadius));

                /*GameObject temp2 = new GameObject("Nav Point");
                temp2.transform.position = this._unitPathing.corners[this._unitPathing.corners.Length - 1];
                GameObject temp = new GameObject("Attack Point");
                temp.transform.position = attackPoint;*/

                this._unitPathing.corners[this._unitPathing.corners.Length - 1] = attackPoint;
            }

            for(int i = 1; i < this._unitPathing.corners.Length; i++) {
                if(Mathf.Abs(this._unitPathing.corners[i].y - this._unitPathing.corners[i - 1].y) <= 0.3f) {
                    float distance = Vector3.Distance(this._unitPathing.corners[i], this._unitPathing.corners[i - 1]);

                    if(this._staminaToUse > distance) {
                        linePoints.Push(this._unitPathing.corners[i]);
                        this._staminaToUse -= distance;
                    } else {
                        Vector3 direction = (this._unitPathing.corners[i] - this._unitPathing.corners[i - 1]).normalized;
                        Vector3 endPoint = this._unitPathing.corners[i - 1] + (direction * this._staminaToUse);
                        linePoints.Push(endPoint);
                        this._staminaToUse = 0;
                        this._hasStamina = false;
                    }
                } else {
                    Vector3 targetPPoint = this._unitPathing.corners[i];
                    Vector3 direction = (targetPPoint - this._unitPathing.corners[i - 1]).normalized;

                    bool reachedNextPoint = false;

                    while(true) {
                        Vector3 previousPoint = linePoints.Peek();
                        float remainingCastDistance = Vector3.Distance(previousPoint, targetPPoint);
                        Vector3 nextCastPoint = previousPoint + (direction * castLerpSize);

                        if(remainingCastDistance < castLerpSize) {
                            reachedNextPoint = true;
                            nextCastPoint = targetPPoint;
                        }

                        Vector3 foundPoint = GetPointOnGround(nextCastPoint);

                        float distanceThisIteration = Vector3.Distance(previousPoint, foundPoint);

                        if(this._staminaToUse > distanceThisIteration) {
                            this._staminaToUse -= distanceThisIteration;
                        } else {
                            linePoints.Pop();
                            nextCastPoint = previousPoint + (direction * this._staminaToUse);
                            foundPoint = GetPointOnGround(nextCastPoint);

                            this._hasStamina = false;
                            this._staminaToUse = 0;
                        }

                        linePoints.Push(foundPoint);

                        if(!this._hasStamina || reachedNextPoint) {
                            break;
                        }
                    }
                }

                if(!this._hasStamina) {
                    break;
                }
            }

            Vector3[] finalLine = GetFinalPoint(linePoints);
            this._currentPoint = finalLine[0];

            return finalLine;
        }

        private Vector3 GetPointOnGround(Vector3 point) {

            RaycastHit hit;
            Ray ray = new Ray(point + (Vector3.up * 4.0f), Vector3.down);

            Physics.Raycast(ray, out hit, 50.0f, GlobalSettings.LayerValues.groundLayer);

            return hit.point;
        }

        private Vector3[] GetFinalPoint(Stack<Vector3> points) {

            Vector3[] temp = points.ToArray();

            for(int i = 0; i < points.Count; i++) {
                temp[i].y += 0.2f;
            }

            return temp;
        }

        private void CheckMovement() {
            // Seocondary Code to calculate the remainging distance. incase the one on the bottom doesnt work.
            /*float distance = 0.0f;
            Vector3[] corners = this._navMeshAgent.path.corners;
            for(int c = 0; c < corners.Length - 1; ++c) {
                distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);

            }*/

            Debug.Log("Remaining Distance: " + this._navMeshAgent.remainingDistance.ToString());
            Debug.Log("Stopping Distance: " + this._navMeshAgent.stoppingDistance.ToString());
            if(this._navMeshAgent.remainingDistance <= this._navMeshAgent.stoppingDistance) {

                if(!this._navMeshAgent.hasPath || this._navMeshAgent.velocity.sqrMagnitude <= 0.0f) {
                    this.StopMoving();

                    if(this._nextState == UnitState.ATTACK || this._nextState == UnitState.SPECIAL) {
                        if(this.TargetInRange()) {
                            this._previousState = this._currentState;
                            this._currentState = this._nextState;
                            this._nextState = UnitState.IDLE;
                            this.StartStateAnimation();
                            return;
                        }
                    }

                    this._previousState = this._currentState;
                    this._currentState = UnitState.IDLE;
                    this._nextState = UnitState.NONE;

                    this.uiBase.UpdateUI();
                    this.Controller.playerSelect.ChangeState(SelectionState.STANDBY);
                }

            } else {

                if(this._navMeshAgent.remainingDistance == this._initialMoveDist)
                    return;

                // Stamina Calculations go here.
            }
        }

        private bool SamplePosition(Vector3 dest, out Vector3 position) {
            NavMeshHit hit;

            if(NavMesh.SamplePosition(dest, out hit, this._allowedMoveImprecision, this.AreaMask)) {
                position = hit.position;
                return true;
            } else {
                position = Vector3.zero;
                return false;
            }
        }
        #endregion

        #region ATTACK
        public virtual void Attack() {
            if(this._projectilePrefab == null) {
                this._currentState = UnitState.ATTACK;
                this.InternalAttack(this.GetDamage());
            } else {
                GameObject temp = Instantiate(this._projectilePrefab);
                Projectile tempProjjectile = temp.GetComponent<Projectile>() as Projectile;

                if(tempProjjectile == null)
                    tempProjjectile = temp.AddComponent<Projectile>();

                tempProjjectile.SetupTarget(this as IHasHealth, this._currentTarget, this._projectileReleasePoint.position, this._projectileSpeed);
            }
        }

        public override bool ReceiveDamage(float damage, IHasHealth enemy) {
            if(this.IsDead)
                return true;

            IUnit attacker = enemy as IUnit;
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

            Debug.Log("Current Target " + this.name + ": Took " + finalDamage.ToString() + " of Damage from - " + enemy.gameObject.name);
            this.RemoveHealth(finalDamage);
            this.uiBase.UpdateUI();

            if(this.CurrentHealth <= 0.0f) {
                if(this.Controller != null)
                    this.Controller.RemoveUnit(this);

                // NOTE: play any death animations, or add in any death effects onto the scene. 
                this.Death();

                UnitPoolManager.instance.Return(this);

                return true;
            }

            return true;
        }

        public override bool ReceiveDamage(float damage, IHasHealth enemy, Vector3 origin) {
            if(this.IsDead)
                return true;

            this._unitSound.PlayImpact();
            this.ReceiveDamage(damage, enemy);
            this.PlayHitAnimations(origin);

            return true;
        }

        protected virtual void InternalAttack(float damage) {
            this.SpawnAttackParticle();

            this._currentTarget.LastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth, this.transform.position);

            this._previousState = this._currentState;
            this._currentState = UnitState.FINISH;
            this._nextState = UnitState.NONE;
            this._canAttack = false;

            this._hasStamina = false;
            this._currentEnergy = 0.0f;
            this.StopMoving();

            this.uiBase.UpdateUI();
            this.Controller.playerSelect.ChangeState(SelectionState.FREE);
        }

        public virtual void ProjectileCollisionEvent() {
            if(this._currentState == UnitState.ATTACK) {
                this.InternalAttack(this.GetDamage());
            }
        }

        protected virtual void SpawnAttackParticle() {
            if(this._projectilePrefab != null) {
                Vector3 relativePos = this.position - this._currentTarget.position;
                Quaternion tempRotation = Quaternion.LookRotation(relativePos);

                ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_RANGE, this._currentTarget.position, tempRotation);
            } else {
                ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_MELEE, this._currentTarget.position);
            }
        }

        public float GetDamage() {
            float damage = UnityEngine.Random.Range(this._minDamage, this._maxDamage);
            return Mathf.Round(damage);
        }

        protected virtual bool TargetInRange() {
            float distance = 0.0f;
            if(this._currentTarget.entityType == EntityType.UNIT) {
                distance = Vector3.Distance(this.position, this._currentTarget.position);
                distance -= ((UnitBase)this._currentTarget).UnitRadius;
            } else {
                Structure.StructureBase structure = this._currentTarget as Structure.StructureBase;
                distance = Vector3.Distance(this.position, Utils.ClosesPointToBounds(structure.ColliderBounds, this.position));
            }

            //Debug.Log("Distance: " + distance.ToString());
            //Debug.Log("Unit Attack Range: " + (this._attackRange + this._unitRadius).ToString());

            if(distance > (this._attackRange + this._unitRadius))
                return false;

            return true;
        }
        #endregion

        #region ANIMATION
        public virtual void SetupAnimation() {
            this._unitAnimator = this.GetComponent<Animator>();
            if(this._unitAnimator == null)
                throw new System.ArgumentNullException("Unit Animator Is Missing");

            this.SetupEventAnimation();
        }

        public virtual void StartStateAnimation() {
            this._lastAttack = Time.timeSinceLevelLoad;
            this.LookAt(this._currentTarget.position);
            this.StopMoving();

            if(this._currentState == UnitState.ATTACK) {
                this._unitAnimator.Play("Attack");
                this._unitSound.PlayAttack();
            }
        }

        public virtual void PlayHitAnimations(Vector3 origin) {
            if(this.IsDead)
                return;

            Vector3 newOrigin = new Vector3(origin.x, 0.0f, origin.z);
            Vector3 newPos = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);

            Vector3 directionNormal = (newOrigin - newPos).normalized;
            float signedAngle = Vector3.SignedAngle(directionNormal, this.transform.forward, Vector3.up);
            float angleInDegrees = (signedAngle + 360.0f) % 360;

            Debug.Log("Angle TO Degrees: " + angleInDegrees.ToString());

            if(angleInDegrees <= UnitValues.FRONT_L || angleInDegrees >= UnitValues.FRONT_R) {
                // FRONt.
                this._unitAnimator.Play("HitFromFront");
                this._pedestalAnimator.Play("HitFromFront");
                //Debug.Log("ATTACKED FRON FRONT");
            } else if(angleInDegrees >= UnitValues.BEHIND_L && angleInDegrees <= UnitValues.BEHIND_R) {
                // BEHIND
                this._unitAnimator.Play("HitFromBehind");
                this._pedestalAnimator.Play("HitFromBehind");
                //Debug.Log("ATTACKED FROM BEHIND");
            } else if(angleInDegrees > UnitValues.FRONT_L && angleInDegrees < UnitValues.BEHIND_L) {
                // LEFT
                this._unitAnimator.Play("HitFromLeft");
                this._pedestalAnimator.Play("HitFromLeft");
                //Debug.Log("ATTACKED FROM LEFT");
            } else if(angleInDegrees > UnitValues.BEHIND_R && angleInDegrees < UnitValues.FRONT_R) {
                // RIGHT
                this._unitAnimator.Play("HitFromRight");
                this._pedestalAnimator.Play("HitFromLeft");
                //Debug.Log("ATTACKED FROM RIGHT");
            } else {
                Debug.LogError("Angle TO Degree Is Out Of Bounds, Produced Result: " + angleInDegrees.ToString());
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
            deathGO.ColorRenderers(this.Controller.PlayerColor);
            Rigidbody[] deathRB = deathGO.GetComponentsInChildren<Rigidbody>() as Rigidbody[];

            foreach(Rigidbody rb in deathRB) {
                rb.AddForceAtPosition(explsioonDirection * this._explosionForce, explosionPosition);
            }

            AudioSource audioSource = deathGO.AddComponent<AudioSource>();
            audioSource.PlayOneShot(this._unitSound.GetDeathSoundclip());

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
    }
}