namespace Player {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct PlayerInfo {


        public int id;
        public bool turnEnded;

        public float goldResource;
        public float resourceLImit;

        public float masteryPoints;
        public float masteryLimit;

        public int unitCount;
        public int unitLimit;
    }
}