using System;
using UnityEngine;

namespace Script {
    public enum CameraZoomMode {
        ZoomLv0,
        ZoomLv1,
    }

    public enum CameraAngleMode {
        Type0,
        Type1,
        Type2,
    }

    public class CameraController : MonoBehaviour {
        public static CameraController Instance { get; private set; }

        private const float ZoomLv0 = 3f;
        private const float ZoomLv1 = 1.4f;

        private const float AngleType0 = 0f;
        private const float AngleType1 = 30f;
        private const float AngleType2 = -95f;

        private const float AngleModeInterval = 4f;

        [SerializeField]
        private Transform followTarget;

        [SerializeField]
        private Transform gimballTransform;

        [SerializeField]
        private Transform cameraTransform;

        private float targetZoomDistance;
        private float zoomDistance;
        private float gimbalZoomOffset;
        private float zoomOffset;

        private float targetGimbalAngle;
        private float gimbalAngle;
        private float gimbalAngleOffset;

        private float targetAngle;
        private float angleOffset;

        private float angleModeTimer;

        private CameraAngleMode angleMode;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            SetZoomLevel(CameraZoomMode.ZoomLv0);
            SetAngleMode(CameraAngleMode.Type0);

            angleModeTimer = AngleModeInterval;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SetAngleMode(CameraAngleMode.Type0);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SetAngleMode(CameraAngleMode.Type1);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SetAngleMode(CameraAngleMode.Type2);
            }

            if (GameManager.Instance.Phase == GamePhase.Race) {
                angleModeTimer = Mathf.Max(0f, angleModeTimer - Time.deltaTime);
                if (angleModeTimer <= 0f) {
                    NextAngleMode();
                    angleModeTimer = AngleModeInterval;
                }
            }
        }

        private void LateUpdate() {
            if (followTarget == null) return;

            var position = transform.localPosition;

            position = position +
                       (followTarget.position - position) * Mathf.Clamp01(0.2f * 60f * Time.unscaledDeltaTime);

            transform.localPosition = position;

            UpdateZoomDistance();
            UpdateGimballAngle();
            UpdateAngle();
        }

        public void SetZoomLevel(CameraZoomMode mode) {
            switch (mode) {
                case CameraZoomMode.ZoomLv0:
                    targetZoomDistance = ZoomLv0;
                    break;
                case CameraZoomMode.ZoomLv1:
                    targetZoomDistance = ZoomLv1;
                    break;
            }
        }

        public void SetAngleMode(CameraAngleMode mode) {
            angleMode = mode;

            switch (mode) {
                default:
                case CameraAngleMode.Type0:
                    targetAngle = AngleType0;
                    gimbalZoomOffset = 0f;
                    break;
                case CameraAngleMode.Type1:
                    targetAngle = AngleType1;
                    gimbalZoomOffset = 4f;
                    break;
                case CameraAngleMode.Type2:
                    targetAngle = AngleType2;
                    gimbalZoomOffset = 8f;
                    break;
            }
        }

        public void SetGimbalAngleOffset(float offset) {
            gimbalAngleOffset = offset;
        }

        public void SetZoomOffset(float offset) {
            zoomOffset = offset;
        }

        public void SetAngleOffset(float offset) {
            angleOffset = offset;
        }

        private void UpdateZoomDistance() {
            zoomDistance = zoomDistance + (targetZoomDistance - zoomDistance) *
                Mathf.Clamp01(0.1f * 60f * Time.unscaledDeltaTime);
            cameraTransform.localPosition = new Vector3(0, 0, -zoomDistance - zoomOffset - gimbalZoomOffset);
        }

        private void UpdateGimballAngle() {
            if (followTarget == null) return;

            var targetPosition = followTarget.position;
            targetPosition.y += 0.5f;

            bool isHit = false;

            for (float angle = 75f; angle > 10f; angle -= 5f) {
                gimballTransform.localRotation = Quaternion.Euler(angle, 0, 0);

                var direction = cameraTransform.position - targetPosition;
                var ray = new Ray(targetPosition, direction);
                if (Physics.Raycast(ray, out var hit, direction.magnitude + 1f, LayerMask.GetMask("Default"))) {
                    targetGimbalAngle = angle;
                    isHit = true;

                    break;
                }
            }

            if (!isHit) {
                targetGimbalAngle = 10f;
            }

            gimbalAngle = gimbalAngle +
                          (targetGimbalAngle - gimbalAngle) * Mathf.Clamp01(0.2f * 60f * Time.unscaledDeltaTime);
            gimballTransform.localRotation = Quaternion.Euler(gimbalAngle + gimbalAngleOffset, 0, 0);
        }

        private void UpdateAngle() {
            float angle = targetAngle + angleOffset;

            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        public void NextAngleMode() {
            switch (angleMode) {
                case CameraAngleMode.Type0:
                    SetAngleMode(CameraAngleMode.Type1);
                    break;
                case CameraAngleMode.Type1:
                    SetAngleMode(CameraAngleMode.Type2);
                    break;
                case CameraAngleMode.Type2:
                    SetAngleMode(CameraAngleMode.Type0);
                    break;
            }
        }
    }
}