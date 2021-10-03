// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using DG.Tweening;
using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class SnatchObject : MonoBehaviour, IGrabbable
    {
        #region Global Members
        [Section("Snatch Object")]

        [SerializeField] protected new Rigidbody2D rigidbody = null;
        [SerializeField] protected PhysicsObject snatch = null;

        [Space(5)]

        [SerializeField, Range(0f, 90f)] protected float snatchAngle = 1f;
        [SerializeField, Range(0f, 100f)] protected float cursorCoef = 1f;

        [SerializeField, Range(0f, 100f)] protected float wobbleForce = 1f;
        [SerializeField] protected bool destroyOnSnatched = false;

        [Section("State")]

        [SerializeField, ReadOnly] protected bool isBeingSnatched = false;
        [SerializeField, ReadOnly] protected PlayerCursor cursor = null;
        [SerializeField, ReadOnly] protected Transform handTransform = null;
        [SerializeField, ReadOnly] protected Vector3 snatchOrigin = Vector3.zero;

        private Vector3 buffer = Vector3.zero;
        #endregion

        #region Behaviour
        public virtual void Grab(PlayerCursor _cursor, HingeJoint2D _joint)
        {
            cursor = _cursor;
            handTransform = _joint.transform;
            isBeingSnatched = true;

            // Kill tween.
            rigidbody.transform.DOKill();

            rigidbody.velocity = Vector2.zero;
            buffer = handTransform.position;
        }

        public virtual void Drop()
        {
            handTransform = null;
            isBeingSnatched = false;

            cursor.RemoveSpeedCoef(this);

            // Wobble.
            Wobble();
        }

        public virtual void Shake()
        {
            Debug.Log("Nothing happens.");
        }

        protected virtual void Wobble()
        {
            Vector3 _angles = rigidbody.transform.rotation.eulerAngles;
            float _rotation = Mathf.Abs(_angles.z);
            if (_rotation > 180f)
                _rotation -= 360f;

            _rotation = _rotation * .8f * -Mathf.Sign(_angles.z);

            if (Mathf.Abs(_rotation) > 1f)
            {
                _angles.z = _rotation;
                rigidbody.transform.DORotate(_angles, .5f, RotateMode.Fast).SetEase(Ease.InOutQuad).OnComplete(Wobble);
            }
        }

        public virtual void Snatch()
        {
            PhysicsObject _snatch = Instantiate(snatch);
            _snatch.transform.position = transform.position;
            _snatch.transform.rotation = Quaternion.identity;

            cursor.SetInteraction(_snatch);
            isBeingSnatched = false;
        }

        // -----------------------

        protected virtual void Update()
        {
            if (isBeingSnatched)
            {
                Vector3 _pos = handTransform.position;
                Vector3 _movement = _pos - buffer;

                float _angle = Mathf.Atan((_movement.magnitude / (transform.position - (Vector3)rigidbody.position).magnitude)
                                          * Mathf.Rad2Deg * -Mathf.Sign(_movement.x));

                rigidbody.transform.Rotate(Vector3.forward, _angle);

                _angle = rigidbody.transform.rotation.eulerAngles.z;
                if (_angle > 180f)
                    _angle -= 360f;

                _angle = Mathf.Abs(_angle);
                if (_angle >= snatchAngle)
                {
                    Snatch();
                }
                else
                {
                    // Cursor coef.
                    float _coef = 1f / (Mathf.Max(1, _angle) * cursorCoef);
                    cursor.SetSpeedCoef(this, _coef);
                }

                buffer = _pos;
            }
        }
        #endregion
    }
}
