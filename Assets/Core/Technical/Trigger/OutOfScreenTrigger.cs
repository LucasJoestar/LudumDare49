// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    public class OutOfScreenTrigger : MonoBehaviour
    {
        #region Behaviour
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PhysicsObject>(out PhysicsObject _object))
            {
                _object.LoseObject(false);
            }
        }
        #endregion
    }
}
