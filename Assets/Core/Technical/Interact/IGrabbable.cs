// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using UnityEngine;

namespace LudumDare49
{
	public interface IGrabbable
    {
        void Grab(PlayerCursor _cursor, HingeJoint2D _joint);

        void Drop();

        void Shake();
    }
}
