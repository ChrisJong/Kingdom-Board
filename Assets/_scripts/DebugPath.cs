namespace Movement {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

    public class DebugPath : MonoBehaviour {

        public NavMeshAgent agent;
        public LineRenderer lineRenderer;

        private void Start() {
            this.agent = this.GetComponent<NavMeshAgent>();

            if(this.lineRenderer == null) {
                this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
                this.lineRenderer.material = new Material(Shader.Find("Sprites/Default")) {
                    color = Color.red
                };

                this.lineRenderer.startWidth = 0.5f;
                this.lineRenderer.endWidth = 0.5f;

                this.lineRenderer.startColor = Color.red;
                this.lineRenderer.endColor = Color.red;
            }else
                this.lineRenderer = this.GetComponent<LineRenderer>();
        } 

        private void OnDrawGizmos() {
            if(agent == null || agent.path == null) {
                return;
            }

            NavMeshPath path = this.agent.path;

            this.lineRenderer.numPositions = path.corners.Length;

            for(int i = 0; i < path.corners.Length; i++) {
                this.lineRenderer.SetPosition(i, path.corners[i]);
            }
        }

    }

}