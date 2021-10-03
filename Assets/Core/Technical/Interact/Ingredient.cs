// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class Ingredient : PhysicsObject
    {
        #region Global Members
        [Section("Ingredient")]

        [SerializeField] private PotionAction action = null;
        public PotionAction Action => action;
        #endregion
    }
}
