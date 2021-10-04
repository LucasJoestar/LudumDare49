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

        [SerializeField, Required] private TextMeshProUGUI text = null;
        private int score = 0;
        #endregion

        public void IncrementScore(int _score)
        {
            score += score;
            text.text = score.ToString();
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
