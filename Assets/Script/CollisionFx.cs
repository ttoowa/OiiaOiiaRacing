using System;
using UnityEngine;

namespace Script {
    public class CollisionFx : MonoBehaviour {
        private OiiaAgent[] followAgents;

        public void SetFollowAgents(OiiaAgent[] agents) {
            followAgents = agents;
            
            Update();
        }

        private void Update() {
            Vector3 position = Vector3.zero;
            foreach (var agent in followAgents) {
                position += agent.transform.position;
            }

            position /= followAgents.Length;

            transform.localPosition = position;
        }
    }
}