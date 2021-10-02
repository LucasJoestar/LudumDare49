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

        [SerializeField, Range(0f, 90f)] private float rotationAngle = 10f;
        [SerializeField, Range(0f, 1000f)] private float rotationSpeed = 200f;

        [SerializeField, Range(0f, 100f)] private float inertiaCoef = 10f;
        #endregion

        #region Behaviour
        private Transform cursor = null;
        private bool isGrabbed = false;

        // -----------------------

        public void Grab(Transform _cursor)
        {
            /*cursor = _cursor;

            rigidbody.isKinematic = true;
            isGrabbed = true;

            rigidbody.MovePosition(cursor.position - (transform.rotation * grabPoint));*/
        }

        public void Drop()
        {
            rigidbody.velocity *= inertiaCoef;

            /*rigidbody.isKinematic = false;
            isGrabbed = false;

            Vector3 _old = transform.position;
            Vector3 _new = cursor.position - (transform.rotation * grabPoint);
            Vector3 _movement = _new - _old;*/

            //rigidbody.AddForceAtPosition(_movement * inertiaCoef, transform.rotation * (transform.position + centerOfMass), ForceMode2D.Impulse);
        }

        private void Update()
        {
            // Update position.
            /*if (isGrabbed)
            {
                Vector3 _old = transform.position;
                Vector3 _new = cursor.position - (transform.rotation * grabPoint);
                Vector3 _movement = _new - _old;

                rigidbody.MovePosition(_new);

                Quaternion _rotation = (_movement.x == 0f)
                                     ? Quaternion.identity
                                     : Quaternion.Euler(0f, 0f, rotationAngle * -Mathf.Sign(_movement.x));

                transform.rotation = Quaternion.RotateTowards(transform.rotation, _rotation, Time.deltaTime * rotationSpeed);
            }*/
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
