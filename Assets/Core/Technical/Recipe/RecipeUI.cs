// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    [CreateAssetMenu(fileName = "DAT_RecipeUI", menuName = "Datas/RecipeUI", order = 150)]
    public class RecipeUI : ScriptableObject
    {
        #region Content
        [Section("RecipeUI")]
        [SerializeField] private Sprite potionIcon = null;
        [SerializeField] private RecipeStep[] steps = new RecipeStep[] { }; 
        #endregion
    }

    [System.Serializable]
    public class RecipeStep
    {
        [Section("Recipe Step")]
        [SerializeField] private Sprite stepIcon = null;
        [SerializeField, Range(0,2)] private int stepAmount = 0; 
    }
}
