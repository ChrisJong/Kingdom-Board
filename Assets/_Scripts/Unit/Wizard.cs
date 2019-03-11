namespace KingdomBoard.Unit {

    using System.Collections.Generic;

    using UnityEngine;

    using Constants;
    using Enum;
    using Helpers;
    using Manager;
    using UI;
    using Utility;

    [RequireComponent(typeof(WizardUI))]
    public sealed class Wizard : UnitBase {

        #region VARIABLE
        [Header("WIZARD")]
        [SerializeField] private float _splashRange = 0.0f;

        public float SplashRange { get { return this._splashRange; } }
        #endregion

        #region CLASS
        public override void Setup() {

            this._splashRange = this._data.splashRange;

            base.Setup();
        }

        public override void Init() {

            this._splashRange = this._data.splashRange;

            base.Init();
        }

        public override bool SetTarget(IHasHealth target) {

            this.Controller.playerUI.RadiusDrawer.DrawAttackRadius((this._splashRange));
            this.Controller.playerUI.RadiusDrawer.Move(target.position);
            this.Controller.playerUI.RadiusDrawer.TurnOn();

            return base.SetTarget(target);
        }
        #endregion

        #region ATTACK
        protected override void InternalAttack(float damage) {
            this.SpawnAttackParticle();

            this._unitSound.PlaySpecial();
            this._currentTarget.LastAttacker = this;
            this._currentTarget.ReceiveDamage(damage, this as IHasHealth, this.position);

            // NOTE: Find units within the splashRadius and calculate damage on the distance from the main attack source.
            var hits = Utils.hitsBuffers;
            int numberofhits = Physics.SphereCastNonAlloc(this.CurrentTarget.position, this._splashRange + this._unitRadius, Vector3.forward, hits, 0.0f, GlobalSettings.LayerValues.unitLayer | GlobalSettings.LayerValues.structureLayer);
            this._hitComparer.position = this._currentTarget.position;
            System.Array.Sort(hits, this._hitComparer);

            Debug.Log("Start Splash Attack To: " + numberofhits.ToString() + " Enemies");

            List<IHasHealth> unitsToAttack = new List<IHasHealth>();

            for(int i = 0; i < hits.Length; i++) {
                var hit = hits[i];

                if(hit.transform == null)
                    continue;

                if(hit.transform == this.transform)
                    continue;

                if(hit.transform == this._currentTarget.transform)
                    continue;

                var hitHasHealth = hit.collider.GetEntity<IHasHealth>();
                if(hitHasHealth == null || hitHasHealth.IsDead)
                    continue;

                if(this.IsAlly(hitHasHealth))
                    continue;

                unitsToAttack.Add(hitHasHealth);

                float distance = Vector3.Distance(this._currentTarget.position, hitHasHealth.position);
                float finalDamage = 0.0f;
                finalDamage = Mathf.Round(damage - ((distance / this._splashRange) * damage));

                hitHasHealth.LastAttacker = this;
                hitHasHealth.ReceiveDamage(finalDamage, this as IHasHealth, this._currentTarget.position);
                //Debug.Log("Current Target (" + hitHasHealth.gameObject.name + "): Took " + finalDamage.ToString() + " of Damage");
            }

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

        protected override void SpawnAttackParticle() {
            ParticlePoolManager.instance.SpawnParticleSystem(ParticleType.IMPACT_WIZARD_ATTACK, this._currentTarget.position);
        }
        #endregion
    }
}