namespace KingdomBoard.Utility {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class LineRenderDrawCircle02 : MonoBehaviour {
        public float Radius { get; private set; }
        public float Width { get; private set; }
        public int Segments { get; private set; }
        public Color lineColour { get; private set; }

        public LineRenderer LineRender { get; private set; }

        private void Start() {
            this.Radius = 10.0f;
            this.Segments = 128;
            this.Width = 0.1f;
            this.lineColour = new Color(0.0f, 1.0f, 0.0f, 1.0f);

            this.Init();

            this.Draw();
        }

        public void Init() {
            this.LineRender = gameObject.GetComponent<LineRenderer>() as LineRenderer;
            this.LineRender.material = new Material(Shader.Find("Particles/Standard Unlit"));

            this.LineRender.startColor = this.lineColour;
            this.LineRender.endColor = this.lineColour;

            this.LineRender.startWidth = this.Width;
            this.LineRender.endWidth = this.Width;

            this.LineRender.positionCount = (this.Segments + 1);
            this.LineRender.useWorldSpace = false;

            this.LineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.LineRender.receiveShadows = false;
            this.LineRender.allowOcclusionWhenDynamic = false;
        }

        public void Draw() {
            float deltaTheta = (float)(2.0 * Mathf.PI) / this.Segments;
            float theta = 0f;

            for(int i = 0; i < this.Segments + 1; i++) {
                float x = this.Radius * Mathf.Cos(theta);
                float z = this.Radius * Mathf.Sin(theta);

                // NOTE: Need to calculate the height of the segment point and position it so it floats slightly above the ground.
                // float y = ???

                Vector3 pos = new Vector3(x, 0.1f, z);
                this.LineRender.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }
    }
}