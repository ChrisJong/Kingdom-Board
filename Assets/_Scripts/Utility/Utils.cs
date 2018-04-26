namespace Utility {

    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Helpers;

    public static class Utils {
        #region VARIABLE
        private static uint _nextPoolID = 0;
        private static readonly RaycastHit[] _hitsBuffer = new RaycastHit[100];

        public static uint nextPoolID { get { return _nextPoolID++; } }
        public static RaycastHit[] hitsBuffers {
            get {
                Array.Clear(_hitsBuffer, 0, _hitsBuffer.Length); // must clear the buffer every time it is requested.
                return _hitsBuffer;
            } }
        #endregion

        #region CLASS
        public static Vector3 GetGroundedPosition(Vector3 position) {
            var ray = new Ray(position + (Vector3.up * 10f), Vector3.down);

            //Debug.Log(LayersHelper.instance.groundLayer);

            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, 20f, 1 << 8)) {
                return position;
            }

            return hit.point;
        }

        public static void ColorRenderers(this GameObject go, Color color, bool ignoreParticleSystem = true) {
            var renderers = go.GetComponents<Renderer>();
            for(int i = 0; i < renderers.Length; i++) {
                if(!ignoreParticleSystem || renderers[i].GetComponent<ParticleSystem>() == null)
                    renderers[i].material.color = color;
            }

            renderers = go.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; i++) {
                if(!ignoreParticleSystem || renderers[i].GetComponent<ParticleSystem>() == null)
                    renderers[i].material.color = color;

            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue)) {
            TValue val = defaultValue;
            dict.TryGetValue(key, out val);
            return val;
        }
        #endregion
    }
}