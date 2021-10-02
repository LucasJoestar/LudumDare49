// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    [CreateAssetMenu(fileName = "DAT_Recipe", menuName = "Datas/Recipe", order = 150)]
	public class Recipe : ScriptableObject
    {
        #region Content
        [Section("Recipe")]
        [SerializeField] private ActionScore[] actionsScore = new ActionScore[] { };
        #endregion

        #region Methods
        public int GetActionScore(ActionPotion _action)
        {
            for (int i = 0; i < actionsScore.Length ; i++)
            {
                if (_action == actionsScore[i].Action)
                    return actionsScore[i].Score; 
            }
            return 0; 
        }

        #endregion
    }

    [System.Serializable]
    public class ActionScore
    {
        [Section("Action Score")]
        [SerializeField] private ActionPotion action = null;
        [SerializeField, Range(-100, 100)] private int score = 0;

        public ActionPotion Action => action;
        public int Score => score; 
    }
}
