using System;
using System.Collections;
using UnityEngine;

namespace Script {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        private GamePhase phase;

        public GamePhase Phase => phase;

        private float raceTime;

        [SerializeField]
        private GameObject[] countLights;

        [SerializeField]
        private AudioSource countSfx;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            phase = GamePhase.Ready;

            StartCoroutine(GameRoutine());
        }

        private void Update() {
            raceTime += Time.deltaTime;
        }

        public void SetPhase(GamePhase phase) {
            this.phase = phase;
        }

        private IEnumerator GameRoutine() {
            yield return null;

            for (int i = 0; i < countLights.Length; i++) {
                countLights[i].SetActive(false);
            }

            CameraController.Instance.SetAngleMode(CameraAngleMode.Type2);

            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / 2.4f) {
                CameraController.Instance.SetAngleOffset(t * 5f);
                yield return null;
            }

            CameraController.Instance.SetAngleMode(CameraAngleMode.Type1);

            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / 2.4f) {
                CameraController.Instance.SetAngleOffset(t * -5f);
                yield return null;
            }
            
            CameraController.Instance.SetAngleMode(CameraAngleMode.Type0);

            const float TempZoomOffset = 3f;
            CameraController.Instance.SetAngleOffset(0f);
            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / 2f) {
                float easingT = (1f - Easing.EaseOutCubic(t));
                CameraController.Instance.SetGimbalAngleOffset(easingT * 20f);
                CameraController.Instance.SetZoomOffset(easingT * 5f + TempZoomOffset);
                yield return null;
            }

            CameraController.Instance.SetGimbalAngleOffset(0f);
            CameraController.Instance.SetZoomOffset(TempZoomOffset);

            SetPhase(GamePhase.CountDown);

            countSfx.Play();

            for (int i = 0; i < countLights.Length; i++) {
                countLights[i].SetActive(true);

                if (i < countLights.Length - 1) {
                    yield return new WaitForSeconds(1f);
                }
            }

            SetPhase(GamePhase.Race);

            OiiaAgentManager.Instance.SetAgentsState(OiiaState.Move);

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime) {
                float easingT = (1f - Easing.EaseInOutSine(t));
                CameraController.Instance.SetZoomOffset(TempZoomOffset * easingT);

                yield return null;
            }
            
            CameraController.Instance.SetZoomOffset(0f);
        }
    }
}