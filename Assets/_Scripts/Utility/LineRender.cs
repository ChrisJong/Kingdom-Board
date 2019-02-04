namespace Utility {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [System.Serializable, RequireComponent(typeof(LineRenderer))]
    public abstract class LineRender : MonoBehaviour, ILineRender {

        [SerializeField] protected Transform _transform;
        [SerializeField] protected GameObject _gameobject;

        [SerializeField] private LineRenderer _lineRenderer = null;

        public LineRenderer lineRenderer { get { return this._lineRenderer; } }

        public virtual void Init() {
            this._transform = this.transform;
            this._gameobject = this.gameObject;

            this._lineRenderer = this.gameObject.GetComponent<LineRenderer>() as LineRenderer;
            this._lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }
        public abstract void Draw();
        public abstract void SetActive(bool state);
    }
}