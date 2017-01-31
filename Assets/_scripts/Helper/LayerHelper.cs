namespace Helper {

    using UnityEngine;

    public class LayerHelper : Extension.SingletonMono<LayerHelper> {
        public LayerMask unitLayer;
        public LayerMask structureLayer;
        public LayerMask ResourceLayer;
        public LayerMask groundLayer;
    }
}