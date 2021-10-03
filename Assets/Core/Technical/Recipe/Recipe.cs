// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace LudumDare49
{
    [CreateAssetMenu(fileName = "DAT_Recipe", menuName = "Datas/Recipe", order = 150)]
	public class Recipe : ScriptableObject
    {
        #region Content
        [Section("Recipe")]

        public RecipeAction[] RecipeActions = new RecipeAction[] { };
        public RecipeAction[] ForbiddenActions = new RecipeAction[] { };

        [Space(5f)]

        [SerializeField, Range(-100, 0)] public int UndesiredActionScore = -20;
        #endregion
    }

    [Serializable]
    public class RecipeAction
    {
        [Section("Action Score")]

        [SerializeField] private string name = "Action";
        [SerializeField] private PotionAction action = null;
        [SerializeField, Range(-100, 100)] private int score = 0;

        public PotionAction Action => action;

        public int Score => score; 
    }
}
