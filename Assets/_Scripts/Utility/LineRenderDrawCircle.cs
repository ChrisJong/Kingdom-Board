﻿namespace Utility {

    using System.Collections;

    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class LineRenderDrawCircle : MonoBehaviour {
        [Range(0, 256)]
        public int segments = 128;
        [Range(0, 5)]
        public float xRadius = 10.0f;
        [Range(0, 5)]
        public float yRadius = 10.0f;
        [Range(0, 5)]
        public float width = 0.1f;
        public Color lineColour = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        
        public LineRenderer LineRender { get; private set; }

        private void Start() {
            this.Init();

            this.Draw();
        }

        private void Init() {
            this.LineRender = gameObject.GetComponent<LineRenderer>() as LineRenderer;
            this.LineRender.material = new Material(Shader.Find("Particles/Standard Unlit"));

            this.LineRender.startColor = this.lineColour;
            this.LineRender.endColor = this.lineColour;

            this.LineRender.startWidth = this.width;
            this.LineRender.endWidth = this.width;

            this.LineRender.positionCount = (this.segments + 1);
            this.LineRender.useWorldSpace = false;

            this.LineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            this.LineRender.receiveShadows = false;
            this.LineRender.allowOcclusionWhenDynamic = false;
        }

        public void Draw() {
            float x;
            //float y;
            float z;

            float angle = 20f;

            for(int i = 0; i < (segments + 1); i++) {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * yRadius;

                this.LineRender.SetPosition(i, new Vector3(x, 0, z));

                angle += (360f / segments);
            }
        }
    }
}