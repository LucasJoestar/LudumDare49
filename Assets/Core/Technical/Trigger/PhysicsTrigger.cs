// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using UnityEngine;

namespace LudumDare49
{
	public abstract class PhysicsTrigger : MonoBehaviour
    {
        #region Global Members
        public abstract void OnTrigger(PhysicsObject _object);
        #endregion
    }
}
