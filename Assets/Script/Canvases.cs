using System;
using UnityEngine;

namespace Script {
    public class Canvases : MonoBehaviour {
        public static Canvases Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }
    }
}