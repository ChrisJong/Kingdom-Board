namespace Utility {

    using System;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.AI;

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
                if(renderers[i].name.Contains("shattered") || renderers[i].name.StartsWith("shattered") || renderers[i].name.EndsWith("shattered"))
                    continue;

                if(!ignoreParticleSystem || renderers[i].GetComponent<ParticleSystem>() == null)
                    renderers[i].material.color = color;
            }

            renderers = go.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; i++) {
                //Debug.Log("Renderer Name: " + renderers[i].name);
                if(renderers[i].name.Contains("shattered") || renderers[i].name.StartsWith("shattered") || renderers[i].name.EndsWith("shattered"))
                    continue;

                if(!ignoreParticleSystem || renderers[i].GetComponent<ParticleSystem>() == null)
                    renderers[i].material.color = color;

            }
        }

        public static bool SamplePosition(Vector3 dest, out Vector3 position, float distanceFromPoint = 1.0f) {
            NavMeshHit hit;

            if(NavMesh.SamplePosition(dest, out hit, distanceFromPoint, NavMesh.AllAreas)) {
                position = hit.position;
                return true;
            } else {
                position = Vector3.zero;
                return false;
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue)) {
            TValue val = defaultValue;
            dict.TryGetValue(key, out val);
            return val;
        }

        public static bool PointInBounds(Bounds b, Vector3 p) {
            Vector3 min = b.min;
            Vector3 max = b.max;

            if(p.x < min.x || p.y < min.y || p.z < min.z)
                return false;

            if(p.x > max.x || p.y > max.y || p.z > max.z)
                return false;

            // Point is inside of bounds
            return true;
        }

        public static Vector3 ClosesPointToBounds(Bounds b, Vector3 p) {
            Vector3 r = p;
            Vector3 min = b.min;
            Vector3 max = b.max;

            r.x = (r.x < min.x) ? min.x : r.x;
            r.y = (r.y < min.x) ? min.y : r.y;
            r.z = (r.z < min.z) ? min.z : r.z;

            r.x = (r.x > max.x) ? max.x : r.x;
            r.y = (r.y > max.x) ? max.y : r.y;
            r.z = (r.z > max.z) ? max.z : r.z;

            return r;
        }

        public static float DistanceToPointToBounds(Vector3 location, Bounds bound) {
            Vector2 point = new Vector2(location.x, location.z);

            // Formula doesnt take into account the height of both the location point and the bounds height.
            //float height = location.y;
            float xMin = bound.min.x;
            float yMin = bound.min.z;
            float xMax = bound.max.x;
            float yMax = bound.max.z;

            if(point.x < xMin) {
                if(point.y < yMin) {
                    Vector2 diff = point - new Vector2(xMin, yMin);
                    return diff.magnitude;
                } else if(point.y < yMax) {
                    Vector2 diff = point - new Vector2(xMin, yMax);
                    return diff.magnitude;
                } else {
                    return xMin - point.x;
                }
            } else if(point.x > xMax) {
                if(point.y < yMin) {
                    Vector2 diff = point - new Vector2(xMax, yMin);
                    return diff.magnitude;
                } else if(point.y > yMax) {
                    Vector2 diff = point - new Vector2(xMax, yMax);
                    return diff.magnitude;
                } else {
                    return point.x - xMax;
                }
            } else {
                if(point.y < yMin) {
                    return yMin - point.y;
                } else if(point.y > yMax) {
                    return point.y - yMax;
                } else {
                    // the location point is within the bounds.
                    return 0.0f;
                }
            }
        }

        // Gets the direction the entity is facing.
        public static Vector3 GetObjectFacingDirection(Transform entity) {
            return new Vector3(Mathf.Sin(entity.rotation.eulerAngles.y * Mathf.Deg2Rad),
                               0,
                               Mathf.Cos(entity.rotation.eulerAngles.y * Mathf.Deg2Rad));
        }

        public static float FindDegree(int x, int y) {
            float value = (float)((Math.Atan2(x, y) / Math.PI) * 180.0f);
            if(value > 0)
                value += 360.0f;

            return value;
        }

        public static string UppercaseFirst(string s) {
            string temp = s.ToLower();

            if(string.IsNullOrEmpty(s))
                return string.Empty;

            char[] c = temp.ToCharArray();
            c[0] = char.ToUpper(c[0]);
            return new string(c);
        }
        #endregion
    }
}