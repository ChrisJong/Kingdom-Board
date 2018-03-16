namespace Manager {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Extension;

    public class AssetManager : SingletonMono<AssetManager> {
        /////////////
        /// UNITS ///
        /////////////

        public GameObject archer;
        public GameObject magician;
        public GameObject warrior;

        /////////////////
        /// Structure ///
        /////////////////

        public GameObject castle;

        //////////
        /// UI ///
        //////////
        public GameObject playerUI;
    }
}