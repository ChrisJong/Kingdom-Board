namespace KingdomBoard.Extension {

    using UnityEngine;

    using Photon.Pun;

    public abstract class SingletonMonoPun<T> : MonoBehaviourPun where T : SingletonMonoPun<T> {
        
        public static T instance { get; private set; }

        protected virtual void Awake() {
            if(instance != null) {
                Debug.LogWarning(this.ToString() + " another instance is already on the scene, deleting this instance");
                UnityEngine.Object.Destroy(this);
                return;
            }

            instance = this as T;
        }

        public abstract void Init();
    }
}