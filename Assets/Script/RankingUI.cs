using System;
using TMPro;
using UnityEngine;

namespace Script {
    public class RankingUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI currentRankingText;

        [SerializeField]
        private TextMeshProUGUI totalCountText;

        private float scale;

        private void Update() {
            string currentRanking = OiiaAgentManager.Instance.LocalAgent.Ranking.ToString();

            if (currentRankingText.text != currentRanking) {
                currentRankingText.text = currentRanking;
                scale = 1.5f;
            }

            totalCountText.text = $"/{OiiaAgentManager.Instance.AgentList.Count}";

            scale = scale + (1f - scale) * Mathf.Clamp01(0.15f * 60f * Time.unscaledDeltaTime);
            currentRankingText.transform.localScale = Vector3.one * scale;
        }
    }
}