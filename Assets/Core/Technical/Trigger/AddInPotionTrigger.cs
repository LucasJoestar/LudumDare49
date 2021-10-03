// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class AddInPotionTrigger : PhysicsTrigger
    {
        #region Global Members
        [Section("Add-in-Potion Trigger")]

        [SerializeField, Required] private Potion potion = null;
        #endregion

        #region Behaviour
        public override void OnTrigger(PhysicsObject _object)
        {
            if (_object is Ingredient _ingredient)
            {
                // Apply action.
                potion.ApplyAction(_ingredient.Action);

                // Disappear.
                Destroy(_ingredient.gameObject);
            }
        }
        #endregion
    }
}
