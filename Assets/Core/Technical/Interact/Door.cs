// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    public class Door : MonoBehaviour, IInteractObject
    {
        #region Global Members
        [Section("Door")]

        [SerializeField, Required] private GameObject activate = null;
        [SerializeField, Required] private AudioClip clip = null;
        #endregion

        #region Behaviour
        public void Interact()
        {
            SoundManager.Instance.PlayAtPosition(clip, transform.position);

            activate.SetActive(true);
            gameObject.SetActive(false);
        }
        #endregion
    }
}
