using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script {
    public class OiiaAgent : MonoBehaviour {
        private const float DirectingCooldown = 8f;
        private const float CollideCooldown = 0.3f;

        [SerializeField]
        private GameObject idle;

        [SerializeField]
        private GameObject move;

        [SerializeField]
        private Transform rotatePivot;

        [SerializeField]
        private AudioSource sfxPlayer;

        [SerializeField]
        private AudioClip[] collideSfxClips;

        [SerializeField]
        private GameObject oiiaSound;

        [SerializeField]
        private bool isLocal;

        private OiiaAgent nearAgent;

        private Rigidbody rigidbody;

        private OiiaState state;

        private Vector3 prevPosition;

        private float angle;

        private float collideCooldownTimer;
        private float directingCooldownTimer;

        private bool onDirecting;

        private int ranking;

        private float oiiaRandomTimer;

        public bool IsLocal => isLocal;

        public int Ranking => ranking;

        private void Awake() {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            OiiaAgentManager.Instance.AddAgent(this);
            if (isLocal) {
                OiiaAgentManager.Instance.SetLocalAgent(this);
            }

            oiiaRandomTimer = Random.Range(0f, 3f);
            oiiaSound.SetActive(false);

            SetState(OiiaState.Idle);
        }

        private void Update() {
            nearAgent = OiiaAgentManager.Instance.GetNearAgent(this);

            UpdateMove();
            UpdateTransform();

            if (Input.GetKeyDown(KeyCode.T)) {
                SetState(OiiaState.Move);
            }

            collideCooldownTimer = Mathf.Max(0f, collideCooldownTimer - Time.unscaledDeltaTime);
            directingCooldownTimer = Mathf.Max(0f, directingCooldownTimer - Time.unscaledDeltaTime);

            if (state == OiiaState.Move) {
                oiiaRandomTimer = Mathf.Max(0f, oiiaRandomTimer - Time.unscaledDeltaTime);

                if (oiiaRandomTimer <= 0f) {
                    oiiaSound.SetActive(true);
                }
            }
        }

        private void UpdateTransform() {
            if (state != OiiaState.Move) return;

            Vector3 positionDelta = transform.position - prevPosition;
            float positionDeltaDistance = positionDelta.magnitude;

            angle += (Mathf.Abs(positionDeltaDistance) + 0.5f) * 360f * 4f * Time.deltaTime;

            if (angle < -180) {
                angle += 360;
            } else if (angle > 180) {
                angle -= 360;
            }

            rotatePivot.localEulerAngles = new Vector3(0, angle, 0);

            prevPosition = transform.position;
        }

        private void UpdateMove() {
            if (state != OiiaState.Move) return;

            AddForceToRivalAgent(0.15f + ranking * 0.05f);
            AddForceToForward(0.2f + ranking * 0.02f);

            rigidbody.velocity *= 0.99f;
        }

        public void SetState(OiiaState state) {
            this.state = state;

            idle.SetActive(false);
            move.SetActive(false);

            switch (state) {
                case OiiaState.Idle:
                    idle.SetActive(true);
                    break;
                case OiiaState.Move:
                    move.SetActive(true);
                    break;
            }
        }

        public void SetRanking(int ranking) {
            this.ranking = ranking;
        }

        private void OnCollisionStay(Collision other) {
            if (other.collider.attachedRigidbody == null) return;
            var otherRigidbody = other.collider.attachedRigidbody;
            var otherAgent = otherRigidbody.GetComponent<OiiaAgent>();

            if (otherAgent == null) return;

            if (collideCooldownTimer <= 0f && otherAgent.collideCooldownTimer <= 0f) {
                if (!onDirecting && !otherAgent.onDirecting) {
                    if ((isLocal || otherAgent.IsLocal) && directingCooldownTimer <= 0f && otherAgent.directingCooldownTimer <= 0f) {
                        onDirecting = true;
                        otherAgent.onDirecting = true;
                        directingCooldownTimer = DirectingCooldown;
                        otherAgent.directingCooldownTimer = DirectingCooldown;

                        StartCoroutine(CollideDirecting(otherAgent));
                    } else {
                        collideCooldownTimer = CollideCooldown;
                        otherAgent.collideCooldownTimer = CollideCooldown;
                        CollisionOneShot(otherAgent);
                    }
                }
            }
        }

        private void AddForceToRivalAgent(float power, float yPower = 0f) {
            if (nearAgent == null) return;

            var direction = (nearAgent.transform.position - transform.position).normalized;
            direction.y = yPower;
            rigidbody.AddForce(direction * power, ForceMode.Impulse);
        }

        private void AddForceToForward(float power) {
            var direction = new Vector3(0f, 0f, 1f);
            rigidbody.AddForce(direction * power, ForceMode.Impulse);
        }

        private void CollisionOneShot(OiiaAgent otherAgent) {
            AddForceToRivalAgent(Random.Range(-15f, -10f), Random.Range(0f, 8f));

            GameObject collisionFxGameObject = GameObject.Instantiate(FxResource.Instance.collisionFx_OneShot);
            CollisionFx collisionFx = collisionFxGameObject.GetComponent<CollisionFx>();
            collisionFx.SetFollowAgents(new[] { this, otherAgent });

            PlayCollideSfx();

            Destroy(collisionFx, 3f);
        }

        private IEnumerator CollideDirecting(OiiaAgent otherAgent) {
            GameObject collisionFxGameObject = GameObject.Instantiate(FxResource.Instance.collisionFx_Loop);
            CollisionFx collisionFx = collisionFxGameObject.GetComponent<CollisionFx>();
            collisionFx.SetFollowAgents(new[] { this, otherAgent });

            CameraController.Instance.SetZoomLevel(CameraZoomMode.ZoomLv1);

            PlayCollideSfx();

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.3f) {
                Time.timeScale = Mathf.Lerp(1f, 0.3f, t);

                yield return null;
            }

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 2f) {
                AddForceToRivalAgent(0.2f);
                otherAgent.AddForceToRivalAgent(0.2f);

                yield return null;
            }

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.1f) {
                Time.timeScale = Mathf.Lerp(0.3f, 1f, t);

                yield return null;
            }

            CameraController.Instance.SetZoomLevel(CameraZoomMode.ZoomLv0);

            AddForceToRivalAgent(-15f);
            Destroy(collisionFxGameObject);

            Time.timeScale = 1f;

            onDirecting = false;
            otherAgent.onDirecting = false;
        }

        private void PlayCollideSfx() {
            sfxPlayer.clip = collideSfxClips[Random.Range(0, collideSfxClips.Length)];
            sfxPlayer.Play();
        }
    }
}