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
        [SerializeField, Required] private GameObject fx = null;
        [SerializeField, Required] private AudioClip fxClip = null;
        #endregion

        #region Behaviour
        public override void OnTrigger(PhysicsObject _object)
        {
            if (_object is Ingredient _ingredient && _ingredient.CanBeMixedUp)
            {
                // Apply action.
                potion.ApplyAction(_ingredient.Action, true);

                // FX.
                if (fx != null)
                {
                    var _fx = Instantiate(fx);
                    _fx.transform.position = _object.transform.position;
                    _fx.transform.rotation = Quaternion.identity;

                    SoundManager.Instance.PlayAtPosition(fxClip, _object.transform.position);
                }

                // Disappear.
                Destroy(_ingredient.gameObject);
            }
        }
        #endregion
    }
}
