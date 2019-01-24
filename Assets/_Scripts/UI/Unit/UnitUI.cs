namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    using Unit;
    using Player;
    using Utility;

    public abstract class UnitUI : UIBase {

        #region INFO_UI

        private GameObject _infomationGroup;

        [SerializeField] private Image _unitIcon;

        [SerializeField] private GameObject _healthGroup;
        private TextMeshProUGUI _healthText;
        private RectTransform _healthBarTransform;

        [SerializeField] private GameObject _staminaGroup;
        private TextMeshProUGUI _staminaText;
        private RectTransform _staminaBarTransform;

        [SerializeField] private GameObject _LocatorGroup;
        private LineRenderer _LocatorLine;
        private Transform _LocatorMarker;

        [SerializeField] protected GameObject _hoverUI = null;
        [SerializeField] protected RectTransform _hoverUITransform = null;
        [SerializeField] protected Canvas _hoverUICanvas = null;

        [SerializeField]
        private bool _hoverTrigger = false;
        [SerializeField] private bool _selectedTrigger = false;

        public bool HoverTrigger { get { return this._hoverTrigger; } }

        #endregion

        #region VARIABLE
        [SerializeField] protected UnitBase _unitBase;

        [SerializeField] protected List<Renderer> _renderers;
        [SerializeField] protected List<Material> _unitMaterials;

        private LineRenderDrawCircle _radiusDrawer = null;
        #endregion

        #region UNITY
        #endregion

        #region CLASS
        public void Setup(UnitBase unitBase) {
            this._unitBase = unitBase;

            foreach(Renderer renderer in this.GetComponentsInChildren<Renderer>()) {
                this._renderers.Add(renderer);

                foreach(Material mat in renderer.sharedMaterials) {
                    this._unitMaterials.Add(mat);
                }
            }

            if(this._radiusDrawer == null) {
                this._radiusDrawer = this.transform.Find("RadiusDrawer").GetComponent<LineRenderDrawCircle>() as LineRenderDrawCircle;
                this._radiusDrawer.Init();
                this._radiusDrawer.TurnOff();
            } else {
                this._radiusDrawer.Init();
                this._radiusDrawer.TurnOff();
            }

            this.Setup();
        }

        public override void Init(Player controller) {
            base.Init(controller);

            this._infomationGroup = this._controller.playerUI.unitUIGroup;
            this._unitIcon = this._infomationGroup.transform.Find("UnitIcon").Find("Image").GetComponent<Image>() as Image;
            this._healthGroup = this._infomationGroup.transform.Find("BG").Find("Health").gameObject;
            this._healthBarTransform = this._healthGroup.transform.Find("Bar").transform as RectTransform;
            this._healthText = this._healthGroup.transform.Find("Text").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;
            this._staminaGroup = this._infomationGroup.transform.Find("BG").Find("Stamina").gameObject;
            this._staminaBarTransform = this._staminaGroup.transform as RectTransform;
            this._staminaText = this._staminaGroup.transform.Find("Text").GetComponent<TextMeshProUGUI>() as TextMeshProUGUI;

            this._LocatorGroup = this._controller.playerUI.unitLocator;
            this._LocatorLine = this._LocatorGroup.GetComponent<LineRenderer>() as LineRenderer;
            this._LocatorMarker = this._LocatorGroup.transform.Find("Marker").gameObject.transform;

            if(this._hoverUI == null) {
                this._hoverUI = this.transform.Find("_UIHover").gameObject;
                this._hoverUICanvas = this._hoverUI.GetComponent<Canvas>() as Canvas;
                this._hoverUITransform = this._hoverUI.transform as RectTransform;
            }
            this._hoverUICanvas.worldCamera = controller.playerCamera.mainCamera;
            this._hoverUI.SetActive(false);
        }

        public override void Return() {
            this._hoverUICanvas.worldCamera = null;

            this._infomationGroup = null;
            this._unitIcon = null;
            this._healthGroup = null;
            this._healthBarTransform = null;
            this._healthText = null;
            this._staminaGroup = null;
            this._staminaBarTransform = null;
            this._staminaText = null;
            this._LocatorGroup = null;
            this._LocatorLine = null;
            this._LocatorMarker = null;

            base.Return();
        }

        public override void UpdateUI() {
            // information Group
            float healthBarWidth = 290.0f; // Stored Width of the health Bar.
            float newHealth = healthBarWidth * (this._unitBase.CurrentHealth / this._unitBase.MaxHealth);
            Vector2 newRect = new Vector2(newHealth, this._healthBarTransform.rect.height);
            this._healthBarTransform.sizeDelta = newRect;
            this._healthText.text = this._unitBase.CurrentHealth.ToString() + " / " + this._unitBase.MaxHealth.ToString();

            float staminaBarWidth = 290.0f; // Stored Width of the health Bar.
            float newStamina = staminaBarWidth * (this._unitBase.CurrentEnergy / this._unitBase.MaxEnergy);
            Vector2 newStaminaRect = new Vector2(newStamina, this._staminaBarTransform.sizeDelta.y);
            this._staminaBarTransform.sizeDelta = newStaminaRect;
            this._staminaText.text = this._unitBase.CurrentEnergy.ToString() + " / " + this._unitBase.MaxEnergy.ToString();

            this._unitIcon.sprite = this._unitBase.Data.iconSprite;
        }

        public override void Display() {
            Debug.Log("Show Unit UI");

            this.UpdateUI();

            this._selectedTrigger = true;

            this.ActivateOutline(Color.green);
            this._infomationGroup.SetActive(true);
        }

        public override void Hide() {
            this._selectedTrigger = false;

            this.DisableAttackRadius();
            this.DisableMovePath();
            this.DeactivateOutline();
            this._infomationGroup.SetActive(false);
        }

        public override void OnEnter() {
        }

        public override void OnEnter(Player controller) {

            if(this._hoverTrigger)
                return;

            this._hoverTrigger = true;

            Debug.Log("Show Unit Hover UI");

            if(!this._selectedTrigger) {
                if(controller.id == this._controller.id)
                    this.ActivateOutline(Color.blue);
                else
                    this.ActivateOutline(Color.red);
            }

            this._hoverUI.SetActive(true);
        }

        public override void OnExit() {
            if(!this._hoverTrigger)
                return;

            this._hoverTrigger = false;
            if(!this._selectedTrigger)
                this.DeactivateOutline();
            this._hoverUI.SetActive(false);
        }

        public override void ResetUI() {
            this.DeactivateOutline();
            this._hoverUI.SetActive(false);
        }

        public virtual void EnableMovePath(Vector3 point) {

            if(!this._LocatorGroup.activeSelf)
                this._LocatorGroup.SetActive(true);

            Vector3[] path = this._unitBase.ReturnPathToPoint(point);
            if(path != null) {
                this._LocatorLine.enabled = true;
                this._LocatorLine.positionCount = path.Length;
                this._LocatorLine.SetPositions(path);
                this._LocatorMarker.position = path[0];
            }
        }

        public virtual void EnableMovePathToTarget(Vector3 point) {
            if(!this._LocatorGroup.activeSelf)
                this._LocatorGroup.SetActive(true);

            Vector3[] path = this._unitBase.ReturnPathToTarget(point);
            if(path != null) {
                this._LocatorLine.enabled = true;
                this._LocatorLine.positionCount = path.Length;
                this._LocatorLine.SetPositions(path);
                this._LocatorMarker.position = path[0];
                this.MoveAttackRadius(path[0]);
            }
        }

        public virtual void DisableMovePath() {

            this._LocatorLine.positionCount = 0;

            if(this._LocatorGroup.activeSelf)
                this._LocatorGroup.SetActive(false);
        }

        public virtual void EnableAttackRadius() {
            this._radiusDrawer.DrawAttackRadius(this._unitBase.AttackRange + this._unitBase.UnitRadius);
            this._radiusDrawer.MoveToOrigin();
            this._radiusDrawer.TurnOn();
        }

        public virtual void MoveAttackRadius(Vector3 point) {
            this._radiusDrawer.Move(point);
        }

        public virtual void DisableAttackRadius() {
            this._radiusDrawer.MoveToOrigin();
            this._radiusDrawer.TurnOff();
        }

        protected virtual void ActivateOutline(Color color, float width = 0.03f) {
            Debug.Log("Activate outline");

            foreach(Renderer ren in this._renderers) {
                ren.material.SetFloat("_Outline", width);
                ren.material.SetColor("_OutlineColor", color);
            }
        }

        protected virtual void DeactivateOutline() {
            Debug.Log("Deactivate outline");

            foreach(Renderer ren in this._renderers) {
                ren.material.SetFloat("_Outline", 0.0f);
            }
        }





        protected void InitiateAttack() {
        }

        protected void InitiateMove() {
        }

        public virtual void FinishMove() {
        }

        public virtual void FinishAttack() {
        }

        protected virtual void Cancel() {
        }

        protected void End() {
        }
        #endregion
    }
}