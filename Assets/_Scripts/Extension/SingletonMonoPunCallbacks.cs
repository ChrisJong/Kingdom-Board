namespace KingdomBoard.Extension {

    using UnityEngine;

    using Photon.Pun;

    public abstract class SingletonMonoPunCallbacks<T> : MonoBehaviourPunCallbacks where T : SingletonMonoPunCallbacks<T> {
        
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