// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare49
{
	public class PhysicsObject : MonoBehaviour, IGrabbable
    {
        #region Global Members
        [Section("PhysicsObject")]

        [SerializeField, Required] protected new Rigidbody2D rigidbody = null;
        [SerializeField, Required] protected new Collider2D collider = null;

        public Rigidbody2D Rigidbody => rigidbody;
        public Collider2D Collider => collider;

        [SerializeField] protected Vector3 centerOfMass = Vector3.zero;
        [SerializeField, HelpBox("Cyan", MessageType.Info)] protected Vector3 grabPoint = Vector3.zero;

        [Section("Layers")]

        [SerializeField] protected LayerMask physicsMask = new LayerMask();
        [SerializeField] protected LayerMask triggerMask = new LayerMask();

        [Space(5f)]

        [SerializeField] protected LayerMask collisionLayer = new LayerMask();
        [SerializeField] protected LayerMask ignoreLayer = new LayerMask();

        [Section("Settings")]

        [SerializeField, Range(0f, 100f)] protected float inertiaCoef = 10f;

        // -----------------------

        [SerializeField] protected List<Collider2D> ignoredColliders = new List<Collider2D>();
        [SerializeField] protected ContactFilter2D contactFilter = new ContactFilter2D();
        #endregion

        #region Behaviour
        [SerializeField, ReadOnly] protected PhysicsTrigger trigger = null;

        protected bool isGrabbed = false;
        private static Vector2[] positionBuffer = new Vector2[5]; 

        // -----------------------

        public virtual void Grab(PlayerCursor _cursor, HingeJoint2D _joint)
        {
            // Set joint body.
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector2.zero;
            rigidbody.constraints = RigidbodyConstraints2D.None;

            _joint.connectedBody = rigidbody;

            for (int i = 0; i < positionBuffer.Length; i++)
            {
                positionBuffer[i] = transform.position; 
            }

            if (trigger)
            {
                trigger.OnGrabbed(this);
                trigger = null;
            }
            
            ResetIgnoredColliders();
            SetLayer(ignoreLayer);

            isGrabbed = true;
        }

        public virtual void Drop()
        {
            isGrabbed = false;

            Vector2 _dir = positionBuffer[0] - positionBuffer[positionBuffer.Length - 1];
            rigidbody.velocity = _dir * inertiaCoef;

            SetLayer(collisionLayer);
            DoOverlap();
        }

        public virtual void Shake()
        {
            Debug.Log("Shake!");
        }

        protected virtual void Update()
        {
            if (isGrabbed)
            {
                if ((Vector2)transform.position == positionBuffer[0]) return;
                for (int i = positionBuffer.Length; i--> 1;) 
                {
                    positionBuffer[i] = positionBuffer[i - 1]; 
                }
                positionBuffer[0] = transform.position;
            }
            else
            {
                UpdateIgnoredColliders();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + centerOfMass, .1f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + grabPoint, .1f);
        }

        protected virtual void Start()
        {
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;

            SetLayer(collisionLayer);
        }
        #endregion

        #region Layer
        public virtual void SetLayer(LayerMask _layer)
        {
            gameObject.layer = (int)Mathf.Log(_layer.value, 2);
        }
        #endregion

        #region Collision
        protected static Collider2D[] overlapBuffer = new Collider2D[6];

        // -----------------------

        protected virtual void DoOverlap()
        {
            // Physics overlap extraction.
            int _count = OverlapCollider(physicsMask);
            for (int _i = 0; _i < _count; _i++)
            {
                // If overlap, extract from collision.
                Collider2D _collider = overlapBuffer[_i];
                ColliderDistance2D _distance = collider.Distance(_collider);

                if (_distance.isOverlapped)
                    AddIgnoredCollider(_collider);

                //rigidbody.position += _distance.normal * _distance.distance;
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
                if (overlapBuffer[_bestIndex].TryGetComponent(out PhysicsTrigger _trigger))
                {
                    trigger = _trigger;
                    _trigger.OnTrigger(this);
                }
            }
        }

        protected virtual int OverlapCollider(LayerMask _mask)
        {
            contactFilter.layerMask = _mask;
            return collider.OverlapCollider(contactFilter, overlapBuffer);
        }

        protected virtual void AddIgnoredCollider(Collider2D _collider)
        {
            Physics2D.IgnoreCollision(collider, _collider, true);
            ignoredColliders.Add(_collider);
        }

        protected virtual void UpdateIgnoredColliders()
        {
            for (int _i = ignoredColliders.Count; _i-- > 0;)
            {
                Collider2D _collider = overlapBuffer[_i];
                ColliderDistance2D _distance = collider.Distance(_collider);

                if (!_distance.isOverlapped)
                {
                    Physics2D.IgnoreCollision(collider, _collider, false);
                    ignoredColliders.RemoveAt(_i);
                }
            }
        }

        protected virtual void ResetIgnoredColliders()
        {
            foreach (Collider2D _collider in ignoredColliders)
                Physics2D.IgnoreCollision(collider, _collider, false);

            ignoredColliders.Clear();
        }
        #endregion
    }
}
