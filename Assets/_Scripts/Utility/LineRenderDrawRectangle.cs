namespace KingdomBoard.Utility {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class LineRenderDrawRectangle : LineRender {

        public float width = 1.0f;
        public Color lineColour = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        [SerializeField, ReadOnly] private int _segments = 5;
        [SerializeField, ReadOnly] private Vector3[] _setPositions;

        public LineRenderer LineRender { get; private set; }

        private void Awake() {
            this.Init();
        }

        public override void Init() {
            this.LineRender = this.gameObject.GetComponent<LineRenderer>() as LineRenderer;
            this.LineRender.material = new Material(Shader.Find("Particles/Standard Unlit"));

            this.LineRender.startColor = this.lineColour;
            this.LineRender.endColor = this.lineColour;

            this.LineRender.startWidth = this.width;
            this.LineRender.endWidth = this.width;

            this.LineRender.positionCount = this._segments;
            this.LineRender.useWorldSpace = true;

            this.LineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.LineRender.receiveShadows = false;
            this.LineRender.allowOcclusionWhenDynamic = false;

            this._setPositions = new Vector3[this._segments];
        }

        public override void Draw() {
            for(int i = 0; i < this._segments; i++) {
                this.LineRender.SetPosition(i, this._setPositions[i]);
            }
        }

        public void Draw(Bounds bound, float distance) {
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            this._setPositions[0] = new Vector3(min.x - distance, min.y + 0.2f, min.z - distance); // Bottom Left
            this._setPositions[1] = new Vector3(max.x + distance, min.y + 0.2f, min.z - distance); // Bottom Right
            this._setPositions[2] = new Vector3(max.x + distance, min.y + 0.2f, max.z + distance); // Top Right
            this._setPositions[3] = new Vector3(min.x - distance, min.y + 0.2f, max.z + distance); // Top Left
            this._setPositions[4] = new Vector3(min.x - distance, min.y + 0.2f, min.z - distance); // Cap

            this.Draw();
        }

        public void Draw(Bounds bound, float distance, float width = 1.0f) {

            this.LineRender.startWidth = width;
            this.LineRender.endWidth = width;

            this.Draw(bound, distance);
        }

        public override void SetActive(bool state) {
            this.gameObject.SetActive(state);
        }
    }
}