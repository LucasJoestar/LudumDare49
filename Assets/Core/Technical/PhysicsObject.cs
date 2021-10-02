// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class PhysicsObject : MonoBehaviour
    {
        #region Global Members
        [Section("PhysicsObject")]

        [SerializeField, Required] private new Rigidbody2D rigidbody = null;
        [SerializeField, Required] private new Collider2D collider = null;

        [SerializeField] private Vector3 centerOfMass = Vector3.zero;
        [SerializeField, HelpBox("Cyan", MessageType.Info)] private Vector3 grabPoint = Vector3.zero;

        [Space(10f)]

        [SerializeField, Range(0f, 100f)] private float inertiaCoef = 10f;
        #endregion

        #region Behaviour
        private bool isGrabbed = false;
        private static Vector2[] positionBuffer = new Vector2[5]; 
        // -----------------------

        public void Grab()
        {
            rigidbody.velocity = Vector2.zero;
            for (int i = 0; i < positionBuffer.Length; i++)
            {
                positionBuffer[i] = transform.position; 
            }
            isGrabbed = true; 
        }

        public void Drop()
        {
            isGrabbed = false;
            Vector2 _dir = positionBuffer[0] - positionBuffer[positionBuffer.Length - 1];
            rigidbody.velocity = _dir * inertiaCoef;
        }

        private void Update()
        {
            if(isGrabbed)
            {
                if ((Vector2)transform.position == positionBuffer[0]) return;
                for (int i = positionBuffer.Length; i--> 1;) 
                {
                    positionBuffer[i] = positionBuffer[i - 1]; 
                }
                positionBuffer[0] = transform.position;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + centerOfMass, .1f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + grabPoint, .1f);
        }
        #endregion
    }
}
