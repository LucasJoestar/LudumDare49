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

        [SerializeField, Required] protected new Rigidbody2D rigidbody = null;
        [SerializeField, Required] protected new Collider2D collider = null;

        [SerializeField] protected Vector3 centerOfMass = Vector3.zero;
        [SerializeField, HelpBox("Cyan", MessageType.Info)] protected Vector3 grabPoint = Vector3.zero;

        [Section("Settings")]

        [SerializeField] protected LayerMask physicsMask = new LayerMask();
        [SerializeField] protected LayerMask triggerMask = new LayerMask();

        [Space(10f)]

        [SerializeField, Range(0f, 100f)] protected float inertiaCoef = 10f;
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

            DoOverlap();
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

        #region Collision
        protected static Collider2D[] overlapBuffer = new Collider2D[6];
        protected ContactFilter2D contactFilter = new ContactFilter2D();

        // -----------------------

        protected void DoOverlap()
        {
            // Physics overlap extraction.
            int _count = OverlapCollider(physicsMask);
            for (int _i = 0; _i < _count; _i++)
            {
                // If overlap, extract from collision.
                ColliderDistance2D _distance = collider.Distance(overlapBuffer[_i]);

                if (_distance.isOverlapped)
                    rigidbody.position += _distance.normal * _distance.distance;
            }

            // Snap test.
            _count = OverlapCollider(triggerMask);
            if (_count > 0)
            {
                float _bestDistance = 999f;
                int _bestIndex = 0;

                for (int _i = 0; _i < _count; _i++)
                {
                    Collider2D _overlap = overlapBuffer[_i];
                    float _distanceValue = Mathf.Abs(Vector2.Distance(rigidbody.position, _overlap.attachedRigidbody.position));

                    if (_distanceValue < _bestDistance)
                    {
                        _bestDistance = _distanceValue;
                        _bestIndex = _i;
                    }
                }

                // Trigger.
                if (overlapBuffer[_bestIndex].TryGetComponent<PhysicsTrigger>(out PhysicsTrigger _trigger))
                {
                    _trigger.OnTrigger(this);
                }
            }
        }

        protected int OverlapCollider(LayerMask _mask)
        {
            contactFilter.layerMask = _mask;
            return collider.OverlapCollider(contactFilter, overlapBuffer);
        }
        #endregion
    }
}
