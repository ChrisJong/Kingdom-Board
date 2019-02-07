namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    using Constants;
    using Unit;
    using Player;
    using Utility;

    public abstract class UnitUI : UIBase {

        #region VARIABLE
        [Space]
        [SerializeField] private bool _selectedTrigger = false;
        [SerializeField] private bool _hoverTrigger = false;

        [Space]
        [SerializeField] protected UnitBase _unitBase;

        [SerializeField] private LineRenderDrawCircle _radiusDrawer = null;

        [SerializeField, HideInInspector] protected List<Renderer> _renderers;
        [SerializeField, HideInInspector] protected List<Material> _unitMaterials;

        [SerializeField] private GameObject _LocatorGroup;
        private LineRenderer _LocatorLine;
        private Transform _LocatorMarker;

        [Header("UNIT - MAIN UI")]
        [SerializeField] private Image _unitIcon;

        [Space]
        [SerializeField] private GameObject _healthGroup;

        private TextMeshProUGUI _healthText;

        private RectTransform _healthBarTransform;

        [SerializeField] private GameObject _staminaGroup;

        private TextMeshProUGUI _staminaText;

        private RectTransform _staminaBarTransform;

        [Header("UNIT - HOVER UI")]
        [SerializeField] protected GameObject _hoverGroup = null;

        [SerializeField] protected RectTransform _hoverTransform = null;

        [SerializeField] protected Canvas _hoverCanvas = null;

        [Space]
        [SerializeField] protected Image _healthImage = null;
        [SerializeField] protected Image _staminaImage = null;

        public bool HoverTrigger { get { return this._hoverTrigger; } }
        #endregion

        #region CLASS
        public virtual void Setup(UnitBase unitBase) {
            this._unitBase = unitBase;

            this._renderers.Clear();
            this._unitMaterials.Clear();
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

            if(this._hoverGroup == null) {
                this._hoverGroup = this.transform.Find(UIValues.Unit.UNIT_UI_HOVER).gameObject;
                this._hoverCanvas = this._hoverGroup.GetComponent<Canvas>() as Canvas;
                this._hoverTransform = this._hoverGroup.transform as RectTransform;

                this._healthImage = this._hoverTransform.Find("Health" + UIValues.IMAGE_SUFFIX).GetComponent<Image>();
                this._staminaImage = this._hoverTransform.Find("Stamina" + UIValues.IMAGE_SUFFIX).GetComponent<Image>();
            }
        }

        public override void Init(Player controller) {
            base.Init(controller);

            this._mainGroup = this._controller.playerUI.unitUIGroup;
            if(this._mainGroup != null) {
                this._mainRectTransform = this._mainGroup.transform as RectTransform;
                this._mainCanvas = this._mainGroup.GetComponent<Canvas>();

                this._unitIcon = this._mainGroup.transform.Find("Icon_Base").Find("Icon" + UIValues.IMAGE_SUFFIX).GetComponent<Image>() as Image;

                this._healthGroup = this._mainGroup.transform.Find("Base").Find("Health").gameObject;
                this._healthText = this._healthGroup.transform.Find(UIValues.TEXT_SUFFIX).GetComponent<TextMeshProUGUI>();
                this._healthBarTransform = this._healthGroup.transform.Find(UIValues.IMAGE_SUFFIX).transform as RectTransform;

                this._staminaGroup = this._mainGroup.transform.Find("Base").Find("Stamina").gameObject;
                this._staminaText = this._staminaGroup.transform.Find(UIValues.TEXT_SUFFIX).GetComponent<TextMeshProUGUI>();
                this._staminaBarTransform = this._staminaGroup.transform.Find(UIValues.IMAGE_SUFFIX).transform as RectTransform;
            }

            this._LocatorGroup = this._controller.playerUI.unitLocator;
            if(this._LocatorGroup != null) {
                this._LocatorLine = this._LocatorGroup.GetComponent<LineRenderer>() as LineRenderer;
                this._LocatorMarker = this._LocatorGroup.transform.Find("Marker").gameObject.transform;
            }

            this._hoverCanvas.worldCamera = controller.playerCamera.mainCamera;
            this._hoverGroup.SetActive(false);
        }

        public override void Return() {
            this._hoverCanvas.worldCamera = null;

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
            // Main Group
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

        public void UpdateHoverUI() {
            // Hover Group
            float newHealth = UIValues.Unit.FULL_HEALTH * (this._unitBase.CurrentHealth / this._unitBase.MaxHealth);
            float newStamina = UIValues.Unit.FULL_STAMINA * (this._unitBase.CurrentEnergy / this._unitBase.MaxEnergy);

            this._healthImage.fillAmount = newHealth;
            this._staminaImage.fillAmount = newStamina;
        }

        public override void Display() {
            this.UpdateUI();

            this._selectedTrigger = true;

            this.ActivateOutline(Color.green);

            this._mainGroup.SetActive(true);
        }

        public override void Hide() {
            this._selectedTrigger = false;

            this.DisableRadius();
            this.DisableMovePath();
            this.DeactivateOutline();

            this._mainGroup.SetActive(false);
        }

        public override void OnEnter() {}

        public override void OnEnter(Player controller) {

            if(this._hoverTrigger)
                return;

            this._hoverTrigger = true;

            this.UpdateHoverUI();

            Debug.Log("Show Unit Hover UI");

            if(!this._selectedTrigger) {
                if(controller.id == this._controller.id)
                    this.ActivateOutline(Color.blue);
                else
                    this.ActivateOutline(Color.red);
            }

            this._hoverGroup.SetActive(true);
        }

        public override void OnExit() {
            if(!this._hoverTrigger)
                return;

            this._hoverTrigger = false;
            if(!this._selectedTrigger)
                this.DeactivateOutline();

            this._hoverGroup.SetActive(false);
        }

        public override void ResetUI() {
            this.DeactivateOutline();
            this._hoverGroup.SetActive(false);
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

        public virtual void EnableMovePathToTarget(Vector3 point, float range = 0.0f) {
            if(!this._LocatorGroup.activeSelf)
                this._LocatorGroup.SetActive(true);

            Vector3[] path = this._unitBase.ReturnPathToTarget(point, range);
            if(path != null) {
                this._LocatorLine.enabled = true;
                this._LocatorLine.positionCount = path.Length;
                this._LocatorLine.SetPositions(path);
                this._LocatorMarker.position = path[0];
                this.MoveRadius(path[0]);
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

        public virtual void DrawRadius(Color color, float radius) {
            this._radiusDrawer.DrawRadius(color, radius);
            this._radiusDrawer.MoveToOrigin();
            this._radiusDrawer.TurnOn();
        }

        public virtual void MoveRadius(Vector3 point) {
            this._radiusDrawer.Move(point);
        }

        public virtual void DisableRadius() {
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



        protected virtual void Cancel() {
        }

        protected void End() {
        }
        #endregion
    }
}