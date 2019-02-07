namespace Utility {

    using UnityEngine;
    using UnityEditor;
    using System;

    public class LineRenderDrawCircle : LineRender {
        public int segments = 128;

        public float xRadius = 10.0f;
        public float yRadius = 10.0f;
        public float width = 0.1f;

        public Color lineColour = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        private void Awake() {
            this.Init();

            //this.Draw();
        }

        public override void Init() {
            base.Init();

            this.lineRenderer.startColor = this.lineColour;
            this.lineRenderer.endColor = this.lineColour;

            this.lineRenderer.startWidth = this.width;
            this.lineRenderer.endWidth = this.width;

            this.lineRenderer.positionCount = (this.segments + 1);
            this.lineRenderer.useWorldSpace = false;

            this.lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.lineRenderer.receiveShadows = false;
            this.lineRenderer.allowOcclusionWhenDynamic = false;
        }

        public override void Draw() {
            float x;
            //float y;
            float z;

            float angle = 20f;

            for(int i = 0; i < (segments + 1); i++) {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * yRadius;

                this.lineRenderer.SetPosition(i, new Vector3(x, 0, z));

                angle += (360f / segments);
            }
        }

        public void TurnOn() {
            this.gameObject.SetActive(true);
        }

        public void TurnOff() {
            this.gameObject.SetActive(false);
        }

        public void DrawRadius(float radius, float width = 0.1f, int segments = 128) {
            this.xRadius = radius;
            this.yRadius = radius;
            this.width = width;
            this.segments = segments;

            this.lineColour = Color.white;
            this.lineRenderer.startColor = this.lineColour;
            this.lineRenderer.endColor = this.lineColour;

            this.Draw();
        }

        public void DrawRadius(Color color, float radius, float width = 0.1f, int segments = 128) {
            this.xRadius = radius;
            this.yRadius = radius;
            this.width = width;
            this.segments = segments;

            this.lineColour = color;
            this.lineRenderer.startColor = this.lineColour;
            this.lineRenderer.endColor = this.lineColour;

            this.Draw();
        }

        public void DrawAttackRadius(float radius, float width = 0.1f, int segments = 128) {
            this.DrawRadius(Color.red, radius, width, segments);
            this.TurnOn();
        }

        public void DrawMoveRadius(float radius, float width = 0.1f, int segments = 128) {
            this.DrawRadius(Color.blue, radius, width, segments);
        }

        public void DrawSpecialRadius(float radius, float width = 0.1f, int segments = 128) {
            this.DrawRadius(Color.white, radius, width, segments);
        }

        public void DrawRectangle(Color color, Bounds bound, float distance, float width = 0.01f) {
            this.lineColour = color;
            this.segments = 4;

            Vector3 min = bound.min;
            Vector3 max = bound.max;
            Vector3[] positions = new Vector3[4];

            // Bottom Left
            positions[0] = new Vector3(min.x - distance, min.y, min.z - distance);
            // Bottom Right
            positions[1] = new Vector3(max.x + distance, min.y, min.z - distance);
            // Top Right
            positions[2] = new Vector3(max.x + distance, min.y, max.z + distance);
            // Top Left
            positions[3] = new Vector3(min.x - distance, min.y, max.z + distance);

            this.lineRenderer.SetPositions(positions);

        }

        public void Move(Vector3 point) {
            this._transform.position = point;
        }

        public void MoveToOrigin() {
            this._transform.localPosition = Vector3.zero;
        } 

        public void UpdateRadius(float radius = 10.0f) {
            this.xRadius = radius;
            this.yRadius = radius;

            EditorUtility.SetDirty(this);

            this.Draw();
        }

        public void UpdateRadius(float xRadius = 10.0f, float yRadius = 10.0f) {
            this.xRadius = xRadius;
            this.yRadius = yRadius;

            EditorUtility.SetDirty(this);

            this.Draw();
        }

        public override void SetActive(bool state) {
            this.gameObject.SetActive(state);
        }
    }
}