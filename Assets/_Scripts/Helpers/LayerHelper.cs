namespace Helpers {

    using UnityEngine;

    using Extension;

    public sealed class LayersHelper : SingletonMono<LayersHelper> {
        public LayerMask unitLayer;
        public LayerMask resourceLayer;
        public LayerMask structureLayer;
        public LayerMask groundLayer;
    }
}