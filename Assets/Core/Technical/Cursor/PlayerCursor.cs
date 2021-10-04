// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace LudumDare49
{
	public class PlayerCursor : MonoBehaviour
    {
        #region State
        public enum CursorState
        {
            Finger,
            Hand,
            Grab
        }
        #endregion

        #region Hit Comparer
        public class HitComparer : IComparer
        {
            public static readonly HitComparer Comparer = new HitComparer();

            // -----------------------

            int IComparer.Compare(object _a, object _b)
            {
                if (!(_a is RaycastHit2D _aHit) || !_aHit.transform || !_aHit.transform.TryGetComponent<SortingGroup>(out var _aGroup))
                    return 1;

                if (!(_b is RaycastHit2D _bHit) || !_bHit.transform || !_bHit.transform.TryGetComponent<SortingGroup>(out var _bGroup))
                    return -1;

                return _aGroup.sortingOrder.CompareTo(_bGroup.sortingOrder);
            }
        }
        #endregion

        #region Global Members
        [Section("Game Cursor")]

        [SerializeField, Required] private Camera fullCamera = null;
        [SerializeField, Required] private Camera gameCamera = null;

        [Space(5f)]

        [SerializeField, Required] private Transform worldTransform = null;

        public Transform WorldTransform => worldTransform;

        [Space(5f)]

        [SerializeField, Required] private RectTransform canvasTransform = null;
        [SerializeField, Required] private RectTransform rectTransform = null;
        [SerializeField, Required] private Image sprite = null;

        [Space(5f)]

        [SerializeField] private Sprite fingerIcon = null;
        [SerializeField] private Sprite handIcon = null;
        [SerializeField] private Sprite grabIcon = null;

        [Section("Settings")]

        [SerializeField, Range(0f, 10000f)] private float speed = 3000f;
        [SerializeField, ReadOnly] private float currentSpeed = 0f;

        private Dictionary<Object, float> speedCoefs = new Dictionary<Object, float>();

        [Space(5f)]

        [SerializeField] private LayerMask layer = new LayerMask();

        [Section("Shake")]

        [SerializeField, Range(0f, 2f)] private float inactiveResetTime = .5f;
        [SerializeField, Range(0, 20)] private int shakeLoops = 5;

        [Space(5f)]

        [SerializeField, ReadOnly] private int shakeCount = 0;
        [SerializeField, ReadOnly] private float inactiveTime = 0f;

        private Vector3 lastPosition = Vector3.zero;
        private Vector3 lastMovement = Vector3.zero;
        private float lastShakeSign = 0;

        [Section("State")]

        [SerializeField, Required] private HingeJoint2D joint = null;
        [SerializeField, ReadOnly] private IGrabbable interaction = null;

        private CursorState state = CursorState.Finger;

        public CursorState State => state;
        #endregion

        #region Speed
        public void SetSpeedCoef(Object _object, float _coef)
        {
            if (speedCoefs.ContainsKey(_object))
            {
                currentSpeed /= speedCoefs[_object];

                speedCoefs[_object] = _coef;
                currentSpeed *= _coef;
            }
            else
            {
                speedCoefs.Add(_object, _coef);
                currentSpeed *= _coef;
            }
        }

        public void RemoveSpeedCoef(Object _object)
        {
            if (speedCoefs.ContainsKey(_object))
            {
                currentSpeed /= speedCoefs[_object];
                speedCoefs.Remove(_object);
            }
        }
        #endregion

        #region Mono Behaviour
        private readonly RaycastHit2D[] hits = new RaycastHit2D[6];

        // -----------------------

        #if !UNITY_EDITOR
        private void FixedUpdate()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        #endif

        private void Update()
        {
            // Get mouse position.
            Vector2 _mousePosition = fullCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue()) * canvasTransform.sizeDelta;
            Vector2 _offset = canvasTransform.sizeDelta / 2f;

            _offset *= gameCamera.rect.width < 1
                     ? new Vector2(1f - gameCamera.rect.width, 0f)
                     : new Vector2(0f, 1f - gameCamera.rect.height);

            // Clamp position.
            _mousePosition.x = Mathf.Clamp(_mousePosition.x, _offset.x, canvasTransform.sizeDelta.x - _offset.x);
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, _offset.y, canvasTransform.sizeDelta.y - _offset.y);

            rectTransform.anchoredPosition = _mousePosition
                                           = Vector2.MoveTowards(rectTransform.anchoredPosition, _mousePosition, Time.deltaTime * currentSpeed);


            _mousePosition /= canvasTransform.sizeDelta;

            // Set world transform and last position.
            Vector3 _pos = fullCamera.ViewportToWorldPoint(_mousePosition);
            _pos.z = 0f;

            lastPosition = worldTransform.position;
            lastMovement = _pos - lastPosition;

            worldTransform.position = _pos;

            switch (state)
            {
                case CursorState.Finger:
                {
                    // Get interactable objects.
                    _mousePosition = fullCamera.ViewportToScreenPoint(_mousePosition);
                    int _amount = Physics2D.RaycastNonAlloc(gameCamera.ScreenToWorldPoint(_mousePosition), Vector2.zero, hits, 100f, layer);

                    Array.Sort(hits, 0, _amount, HitComparer.Comparer);

                    // Set hand state.
                    if ((_amount > 0) && !hits[0].collider.transform.TryGetComponent<BlockInteract>(out _))
                    {
                        state = CursorState.Hand;
                        sprite.sprite = handIcon;
                    }
                }
                    break;

                case CursorState.Hand:
                {
                    // Get interactable objects.
                    _mousePosition = fullCamera.ViewportToScreenPoint(_mousePosition);
                    int _amount = Physics2D.RaycastNonAlloc(gameCamera.ScreenToWorldPoint(_mousePosition), Vector2.zero, hits, 100f, layer);

                    Array.Sort(hits, 0, _amount, HitComparer.Comparer);

                    #if UNITY_EDITOR
                    for (int _i = 0; _i < _amount; _i++)
                    {
                        GameObject _object = hits[_i].collider.gameObject;
                        Debug.Log("Interact => " + _object.name);
                    }
                    #endif

                    if (_amount == 0)
                    {
                        // Finger state.
                        state = CursorState.Finger;
                        sprite.sprite = fingerIcon;
                    }
                    else
                    {
                        // Grab.
                        if (Mouse.current.leftButton.wasPressedThisFrame)
                        {
                            Transform _transform = hits[0].collider.transform;

                            if (_transform.TryGetComponent<IGrabbable>(out var _grab))
                            {
                                // Grab the object.
                                SetInteraction(_grab);

                                break;
                            }
                            else if (_transform.TryGetComponent<IInteractObject>(out var _interaction))
                            {
                                // Interact.
                                _interaction.Interact();
                            }
                        }
                    }
                }
                    break;

                case CursorState.Grab:
                {
                    // Drop.
                    if (!Mouse.current.leftButton.isPressed)
                    {
                        interaction.Drop();
                        interaction = null;

                        joint.connectedBody = null;

                        state = CursorState.Finger;
                        sprite.sprite = fingerIcon;
                    }
                    else
                    {
                        // Shake.
                        if (lastMovement.y == 0f)
                        {
                            // Reset shake.
                            inactiveTime += Time.deltaTime;
                            if (inactiveTime > inactiveResetTime)
                            {
                                inactiveTime = 0f;
                                shakeCount = 0;
                            }
                        }
                        else
                        {
                            float _shakeSign = Mathf.Sign(lastMovement.y);
                            if (_shakeSign != lastShakeSign)
                            {
                                lastShakeSign = _shakeSign;
                                inactiveTime = 0f;

                                shakeCount++;
                                if (shakeCount == shakeLoops)
                                {
                                    // Shake it.
                                    shakeCount = 0;
                                    interaction.Shake();
                                }
                            }
                        }
                    }
                }
                    break;

                default:
                    break;
            }
        }

        private void Start()
        {
            currentSpeed = speed;
        }

        public void SetInteraction(IGrabbable _interaction)
        {
            if (interaction != null)
            {
                interaction.Drop();
                joint.connectedBody = null;
            }

            interaction = _interaction;
            _interaction.Grab(this, joint);

            state = CursorState.Grab;
            sprite.sprite = grabIcon;

            shakeCount = 0;
            inactiveTime = 0f;
        }
        #endregion
    }
}
