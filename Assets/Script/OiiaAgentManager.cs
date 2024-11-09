using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script {
    public class OiiaAgentManager : MonoBehaviour {
        public static OiiaAgentManager Instance { get; private set; }

        private List<OiiaAgent> agentList = new List<OiiaAgent>();

        private OiiaAgent localAgent;
        
        public List<OiiaAgent> AgentList => agentList;
        
        public OiiaAgent LocalAgent => localAgent;
        
        private void Awake() {
            Instance = this;
        }

        private void Update() {
            UpdateAgentsRanking();
        }

        public void AddAgent(OiiaAgent agent) {
            agentList.Add(agent);
        }
        
        public void SetLocalAgent(OiiaAgent agent) {
            localAgent = agent;
        }
        
        public OiiaAgent GetNearAgent(OiiaAgent agent) {
            OiiaAgent nearAgent = null;
            float minDistance = float.MaxValue;
            foreach (var a in agentList) {
                if (a == agent) continue;
                
                float distance = Vector3.Distance(agent.transform.position, a.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    nearAgent = a;
                }
            }
            
            return nearAgent;
        }
        
        public void SetAgentsState(OiiaState state) {
            foreach (var agent in agentList) {
                agent.SetState(state);
            }
        }

        private void UpdateAgentsRanking() {
            // z-axis sorting 후 agent.SetRanking 으로 순위 업데이트
            
            agentList.Sort((a, b) => {
                if (a.transform.position.z > b.transform.position.z) return -1;
                if (a.transform.position.z < b.transform.position.z) return 1;
                return 0;
            });

            for (int i = 0; i < agentList.Count; i++) {
                agentList[i].SetRanking(i + 1);
            }
        }
    }
}