// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using DG.Tweening;
using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LudumDare49
{
    public class PhysicsObject : MonoBehaviour, IGrabbable
    {
        #region Global Members
        [Section("PhysicsObject")]

        [SerializeField, Required] protected new Rigidbody2D rigidbody = null;
        [SerializeField, Required] protected new Collider2D collider = null;
        [SerializeField, Required] protected AudioSource carrierPrefab = null;
        [SerializeField] private SortingGroup group = null;
        [SerializeField] private AudioClip shakeAudio = null;

        public Rigidbody2D Rigidbody => rigidbody;
        public Collider2D Collider => collider;

        [SerializeField, HelpBox("Cyan", MessageType.Info)] protected Vector3 grabPoint = Vector3.zero;

        [Section("Destroy FX")]

        [SerializeField] private ParticleSystem destroyFX = null;
        [SerializeField] private AudioClip destroyFXClip = null;

        [Section("Layers")]

        [SerializeField] protected LayerMask physicsMask = new LayerMask();
        [SerializeField] protected LayerMask triggerMask = new LayerMask();

        [Space(5f)]

        [SerializeField] protected LayerMask collisionLayer = new LayerMask();
        [SerializeField] protected LayerMask ignoreLayer = new LayerMask();

        [Section("Settings")]

        [SerializeField, Range(0f, 100f)] protected float inertiaCoef = 10f;
        [SerializeField] protected bool doRepop = true;
        [SerializeField, Range(1f, 10f)] private float repopSpeed = 2f;

        [Space(5f)]

        [SerializeField] protected Transform repopPosition = null;
        [SerializeField] protected Transform conveyDefaultPosition = null;
        [SerializeField] protected Transform carrierAnchor = null;
        [SerializeField] protected Transform[] carrierDepopPositions = new Transform[] { };

        [Space(5f)]

        [SerializeField, ReadOnly] protected bool isRepoping = false;
        [SerializeField, ReadOnly] protected AudioSource carrier = null;

        // -----------------------

        protected List<Collider2D> ignoredColliders = new List<Collider2D>();
        protected ContactFilter2D contactFilter = new ContactFilter2D();
        #endregion

        #region Behaviour
        [SerializeField, ReadOnly] protected PhysicsTrigger trigger = null;

        private static Vector2[] positionBuffer = new Vector2[5];
        protected bool isGrabbed = false;
        protected Sequence repopSequence = null;

        public bool IsGrabbed => isGrabbed;

        // -----------------------

        public virtual void Grab(PlayerCursor _cursor, HingeJoint2D _joint)
        {
            // Stop repop.
            if (isRepoping)
            {
                repopSequence.Kill(false);
                OnEndRepop();
            }

            // Set joint body.
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector2.zero;
            rigidbody.constraints = RigidbodyConstraints2D.None;

            _joint.connectedBody = rigidbody;
            _joint.connectedAnchor = grabPoint;

            group.sortingOrder += 5;

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

            group.sortingOrder -= 5;

            SetLayer(collisionLayer);
            DoOverlap();
        }

        public virtual void Shake()
        {
            // Sound.
            if (shakeAudio != null)
                SoundManager.Instance.PlayAtPosition(shakeAudio, transform.position);
        }

        public virtual void Snap()
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public virtual void Eat()
        {
            OnDestroyed();
            LoseObject(true);
        }

        public virtual void LoseObject(bool _resetPos = true)
        {
            if (isRepoping)
                return;

            if (doRepop)
            {
                // Repop.
                if (_resetPos)
                    transform.position = conveyDefaultPosition.position;

                Repop();
            }
            else
            {
                // Destroyed.
                OnDestroyed();
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroyed()
        {
            if (destroyFX != null)
            {
                var _fx = Instantiate(destroyFX);
                _fx.transform.position = transform.position;
                _fx.transform.rotation = Quaternion.identity;

                SoundManager.Instance.PlayAtPosition(destroyFXClip, transform.position);
            }
        }

        protected virtual void Repop()
        {
            isRepoping = true;
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector2.zero;
            rigidbody.rotation = 0f;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            transform.rotation = Quaternion.identity;
            group.sortingOrder += 5;

            // Carrier.
            carrier = Instantiate(carrierPrefab);
            carrier.transform.position = carrierAnchor.position;

            float _duration = Vector2.Distance(transform.position, repopPosition.position) / repopSpeed;
            Vector3 _offset = carrierAnchor.position - transform.position;

            // Repop sequence.
            repopSequence = DOTween.Sequence();

            repopSequence.AppendInterval(1f);
            repopSequence.Join(carrier.DOFade(1f, 1f));
            repopSequence.Append(transform.DOMove(repopPosition.position, _duration).SetEase(Ease.OutQuad));
            repopSequence.Join(carrier.transform.DOMove(repopPosition.position + _offset, _duration).SetEase(Ease.OutQuad));

            Vector3 _scale = carrier.transform.localScale;
            _scale.x = Mathf.Abs(_scale.x) * Mathf.Sign(transform.position.x - repopPosition.position.x);
            carrier.transform.localScale = _scale;

            repopSequence.OnComplete(OnEndRepop);
            repopSequence.Play();
        }

        protected virtual void OnEndRepop()
        {
            DepopCarrier();

            isRepoping = false;
            rigidbody.isKinematic = false;
            rigidbody.constraints = RigidbodyConstraints2D.None;
            group.sortingOrder -= 5;
        }

        protected void DepopCarrier()
        {
            Vector3 _destination = carrierDepopPositions[Random.Range(0, carrierDepopPositions.Length)].position;
            float _duration = Vector2.Distance(transform.position, _destination) / (repopSpeed * 3f);

            AudioSource _carrier = carrier;

            Vector3 _scale = _carrier.transform.localScale;
            _scale.x = Mathf.Abs(_scale.x) * Mathf.Sign(transform.position.x - _destination.x);
            _carrier.transform.localScale = _scale;

            _carrier.transform.DOMove(_destination, _duration).SetEase(Ease.InQuad).OnComplete(() => _carrier.DOFade(0f, 1f).OnComplete(() => Destroy(_carrier.gameObject)));
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + grabPoint, .1f);
        }

        protected virtual void Awake()
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
            _count = OverlapCollider(triggerMask, true);
            if (_count > 0)
            {
                float _bestDistance = 999f;
                int _bestIndex = -1;

                for (int _i = 0; _i < _count; _i++)
                {
                    Collider2D _overlap = overlapBuffer[_i];
                    float _distanceValue = Mathf.Abs(Vector2.Distance(rigidbody.position, _overlap.attachedRigidbody.position));

                    if (_overlap.isTrigger && (_distanceValue < _bestDistance))
                    {
                        _bestDistance = _distanceValue;
                        _bestIndex = _i;
                    }
                }

                // Trigger.
                if ((_bestIndex > -1) && overlapBuffer[_bestIndex].TryGetComponent(out PhysicsTrigger _trigger))
                {
                    trigger = _trigger;
                    _trigger.OnTrigger(this);
                }
            }
        }

        protected virtual int OverlapCollider(LayerMask _mask, bool _useTrigger = false)
        {
            contactFilter.layerMask = _mask;
            contactFilter.useTriggers = _useTrigger;

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
                if (_collider == collider)
                    continue;

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
