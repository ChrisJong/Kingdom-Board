namespace Constants {

    using UnityEngine;

    public static class GlobalSettings {

        public static class Debugging {
            public static bool lockedTraining = false;
        }

        public static class LayerValues {
            public static LayerMask enviromentLayer = 1<<8;
            public static LayerMask unitLayer = 1<<9;
            public static LayerMask structureLayer = 1<<10;
            public static LayerMask groundLayer = 1<<11;
        }
    }
}