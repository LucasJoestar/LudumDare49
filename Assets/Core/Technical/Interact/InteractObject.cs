// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events; 

namespace LudumDare49
{
	public interface IInteractObject
    {
        #region Global Members
        //[Section("InteractObject")]
        #endregion

        #region Behaviour
        public void Interact();
        #endregion
    }
}
