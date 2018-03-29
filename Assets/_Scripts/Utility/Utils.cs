namespace Utility {

    using UnityEngine;

    using Helpers;

    public static class Utils {
        #region VARIABLE
        private static uint _nextPoolID = 0;
        #endregion

        #region CLASS
        public static uint nextPoolID {
            get { return _nextPoolID++; }
        }

        public static Vector3 GetGroundedPosition(Vector3 position) {
            var ray = new Ray(position + (Vector3.up * 10f), Vector3.down);

            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, 20f, LayersHelper.instance.groundLayer)) {
                return position;
            }

            return hit.point;
        }
        #endregion
    }
}