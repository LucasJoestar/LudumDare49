// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using DG.Tweening; 

namespace LudumDare49
{
	public class Lever : MonoBehaviour, IInteractObject
    {
        #region Global Members
        [Section("Lever")]
        [SerializeField] private Dispenser linkedDispenser = null;
        #endregion

        #region Methods
        void IInteractObject.Interact() => linkedDispenser.ActivateDispenser(transform);
        #endregion
    }
}
