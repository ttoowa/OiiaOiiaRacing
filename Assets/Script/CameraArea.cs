using System;
using UnityEngine;

namespace Script {
    public class CameraArea : MonoBehaviour {
        [SerializeField]
        private Transform camera;

        private void Start() {
            camera.gameObject.SetActive(false);
        }

        private void LateUpdate() {
            if (!camera.gameObject.activeSelf) return;

            var localAgent = OiiaAgentManager.Instance.LocalAgent;

            // Look at the local agent
            camera.LookAt(localAgent.transform);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.attachedRigidbody == null) return;
            var otherAgent = other.attachedRigidbody.GetComponent<OiiaAgent>();
            if (otherAgent == null) return;
            if (!otherAgent.IsLocal) return;

            Cameras.Instance.SetCurrentCamera(camera.GetComponent<Camera>());
            Cameras.Instance.MainCamera.gameObject.SetActive(false);
            camera.gameObject.SetActive(true);
        }

        private void OnTriggerExit(Collider other) {
            if (other.attachedRigidbody == null) return;
            var otherAgent = other.attachedRigidbody.GetComponent<OiiaAgent>();
            if (otherAgent == null) return;
            if (!otherAgent.IsLocal) return;

            Cameras.Instance.ResetCurrentCamera();
            Cameras.Instance.MainCamera.gameObject.SetActive(true);
            camera.gameObject.SetActive(false);
        }
    }
}