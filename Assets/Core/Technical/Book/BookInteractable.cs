// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using System; 

namespace LudumDare49
{
    public class BookInteractable : MonoBehaviour, IInteractObject
    {
        #region Global Members
        //[Section("BookInteractables")]
        public event Action OnInteract; 
        #endregion
        public void Interact()
        {
            OnInteract?.Invoke(); 
        }
    }
}
