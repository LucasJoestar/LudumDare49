// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using TMPro;
using UnityEngine;

namespace LudumDare49
{
	public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance = null;

        #region Global Members
        [Section("ScoreManager")]

        [SerializeField, Required] protected AudioClip successClip = null;
        [SerializeField, Required] protected AudioClip failureClip = null;

        [Space(5f)]

        [SerializeField, Required] private TextMeshProUGUI text = null;
        private int score = 0;
        #endregion

        public void IncrementScore(int _score)
        {
            SoundManager.Instance.PlayAtPosition(_score > 0 ? successClip : failureClip, transform.position);

            score += _score;
            text.text = score.ToString();
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
