namespace KingdomBoard.Extension {

    using UnityEngine;

    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T> {

        public static T instance { get; private set; }

        protected virtual void Awake() {
            if(instance != null) {
                Debug.LogWarning(this.ToString() + " another instance is already on the scene, deleting this instance");
                Destroy(this);
                return;
            }

            instance = this as T;
        }

        public abstract void Init();
    }
}