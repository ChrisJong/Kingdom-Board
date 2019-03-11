namespace KingdomBoard.Helpers {

    using System;

    using UnityEngine;

    public static class CircleHelpers {

        public static Vector3 GetPointOnCircle(Vector3 position, float radius, float anglePerSpawn, uint index) {
            var max = 360f / anglePerSpawn;
            var ang = (index % max) * anglePerSpawn;

            float zPos = Math.Sign(position.z) == 1 ? position.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad) : position.z - radius * Mathf.Cos(ang * Mathf.Deg2Rad);

            return new Vector3(
                    position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad),
                    position.y,
                    zPos);
        }
    }
}