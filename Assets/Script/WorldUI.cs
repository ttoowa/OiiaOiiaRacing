using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum WorldUIFollowTarget {
    Position,
    Transform
}

public class WorldUI : MonoBehaviour {
    public WorldUIFollowTarget followTarget;

    public Transform targetTransform;

    public Vector3 targetPosition;

    public Vector3 worldOffset;
    public Vector2 randomOffset;
    public bool destroyWithTargetTransform;

    private RectTransform rectTransform;

    private Canvas parentCanvas;
    private RectTransform parentCanvasRect;

    private Vector2 randomOffsetValue;

    private void Awake() {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.parentCanvas = this.GetComponentInParent<Canvas>();
        this.parentCanvasRect = this.parentCanvas.GetComponent<RectTransform>();

        this.randomOffsetValue = new Vector2(Random.Range(-this.randomOffset.x, this.randomOffset.x),
            Random.Range(-this.randomOffset.y, this.randomOffset.y));
    }

    private void Start() {
        this.SetupAnchor();
        this.UpdatePosition();
    }

    private void Update() {
        this.UpdatePosition();

        if (this.followTarget == WorldUIFollowTarget.Transform && this.destroyWithTargetTransform &&
            this.targetTransform == null) {
            Destroy(this.gameObject);
        }
    }

    public void SetTargetTransform(Transform targetTransform) {
        this.followTarget = WorldUIFollowTarget.Transform;
        this.targetTransform = targetTransform;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        this.followTarget = WorldUIFollowTarget.Position;
        this.targetPosition = targetPosition;
    }

    private void UpdatePosition() {
        Camera targetCamera = Cameras.Instance.CurrentCamera;

        Vector3 actualTargetPosition = Vector3.zero;

        if (this.followTarget == WorldUIFollowTarget.Transform) {
            if (this.targetTransform != null) {
                actualTargetPosition = this.targetTransform.position;
            }
        } else {
            actualTargetPosition = this.targetPosition;
        }

        Vector3 FromCamDir = actualTargetPosition - targetCamera.transform.position;
        Vector3 forward = targetCamera.transform.forward;
        float dotProduct = Vector3.Dot(FromCamDir, forward);

        if (dotProduct < 0) {
            this.Hide();
            return;
        }

        Vector3 targetPosition = actualTargetPosition + this.worldOffset;
        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(targetPosition);

        Vector2 position = new((viewportPosition.x * this.parentCanvasRect.sizeDelta.x) + this.randomOffsetValue.x,
            (viewportPosition.y * this.parentCanvasRect.sizeDelta.y) + this.randomOffsetValue.y);
        this.rectTransform.anchoredPosition = position;
    }

    private void Hide() {
        // GameObject를 끄면 Update가 동작하지 않으므로 anchoredPosition을 변경하여 화면 밖으로 보냄
        this.rectTransform.anchoredPosition = new Vector2(-100000, -100000);
    }

    private void SetupAnchor() {
        this.rectTransform.anchorMin = Vector2.zero;
        this.rectTransform.anchorMax = Vector2.zero;
    }
}