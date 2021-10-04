// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    public class PickupLine : MonoBehaviour, IGrabbable
    {
        #region Global Members
        [Section("PickupLine")]
        [SerializeField, ReadOnly] private Transform handTransform = null;
        private bool isGrabbed = false;
        #endregion
        public void Drop()
        {
            isGrabbed = false;
        }

        public void Grab(PlayerCursor _cursor, HingeJoint2D _joint)
        {
            isGrabbed = true;
            handTransform = _joint.transform;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        public void Shake()
        {
        }

        private void Update()
        {
            if (isGrabbed)
            {
                transform.position = handTransform.position;
            }
        }
    }
}
