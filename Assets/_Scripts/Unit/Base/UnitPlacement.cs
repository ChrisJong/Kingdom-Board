namespace Unit {

    using UnityEngine;

    using Helpers;

    public class UnitPlacement : ObjectPoolBase, IUnitPlacement {

        #region VARIABLE
        private bool _isSetup = false;

        [SerializeField] private Enum.UnitType _unitType = Enum.UnitType.NONE; 

        [SerializeField] private Transform _transfrom;

        [SerializeField] private Renderer[] _renderers;

        private Color _valld = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        private Color _invalid = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        private Color _invisible = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        public bool IsSetup { get { return this._isSetup; } }
        public Enum.UnitType unitType { get { return this._unitType; } set { this._unitType = value; } }

        #endregion

        #region UNITY

        private void Awake() {
            this.Setup();
        }

        #endregion

        #region CLASs

        public void Setup() {

            this._transfrom = this.transform;

            this._renderers = this.GetComponentsInChildren<Renderer>() as Renderer[];

            this._isSetup = true;
        }

        public void Return() {

        }

        public void ChangeColor(bool valid) {
            for(int i = 0; i < this._renderers.Length; i++)
                this._renderers[i].material.color = valid ? this._valld : this._invalid;
        } 

        public void SetPlacement(Vector3 point, Vector3 castlePosition) {
            Vector3 direction = point - castlePosition;

            this._transfrom.position = point;
            this._transfrom.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        public void Hide() {
            //this._transfrom.position = new Vector3(0.0f, -1000.0f, 0.0f);

            for(int i = 0; i < this._renderers.Length; i++)
                this._renderers[i].material.color = this._invisible;
        }

        public void Finished() {
            Manager.UnitPoolManager.instance.Return(this);
        }

        #endregion
    }
}