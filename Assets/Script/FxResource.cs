using UnityEngine;

namespace Script {
    public class FxResource : MonoBehaviour {
        public static FxResource Instance { get; private set; }

        public GameObject collisionFx_Loop;

        public GameObject collisionFx_OneShot;

        private void Awake() {
            Instance = this;
        }
    }
}