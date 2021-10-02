// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class Potion : MonoBehaviour
    {
        #region Global Members
        [Section("Potion")]
        [SerializeField] private Recipe potionRecipe = null;
        [SerializeField, ReadOnly] private int score = 0;
        public int Score => score; 
        #endregion

        #region Methods
        public void ApplyAction(ActionPotion _action) => score += potionRecipe.GetActionScore(_action);
        #endregion
    }
}
