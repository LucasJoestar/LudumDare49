// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
                    return -1;

                if (!(_b is RaycastHit2D _bHit) || !_bHit.transform || !_bHit.transform.TryGetComponent<SortingGroup>(out var _bGroup))
                    return 1;

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

        [Space(5f)]

        [SerializeField, Required] private RectTransform canvasTransform = null;
        [SerializeField, Required] private RectTransform rectTransform = null;
        [SerializeField, Required] private Image sprite = null;

        [Space(5f)]

        [SerializeField] private Sprite fingerIcon,
                                        handIcon,
                                        grabIcon = null;

        [Section("Settings")]

        [SerializeField, Range(0f, 10000f)] private float speed = 3000f;
        [SerializeField] private LayerMask layer = new LayerMask();

        [Section("State")]

        [SerializeField, Required] private HingeJoint2D joint = null;
        [SerializeField, ReadOnly] private PhysicsObject interaction = null;
        private CursorState state = CursorState.Finger;
        #endregion

        #region Mono Behaviour
        private readonly RaycastHit2D[] hits = new RaycastHit2D[6];

        // -----------------------

        //#if UNITY_EDITOR
        private void FixedUpdate()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        //#endif

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
                                           = Vector2.MoveTowards(rectTransform.anchoredPosition, _mousePosition, Time.deltaTime * speed);


            _mousePosition /= canvasTransform.sizeDelta;

            // Set world transform.
            Vector3 _pos = fullCamera.ViewportToWorldPoint(_mousePosition);
            _pos.z = 0f;

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
                    if ((_amount > 0) && !hits[0].transform.TryGetComponent<BlockInteract>(out _))
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
                        Debug.LogWarning("Interact => " + _object.name);
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
                            Transform _transform = hits[0].transform;
                            if (_transform.TryGetComponent<PhysicsObject>(out var _object))
                            {
                                // Grab the object.
                                interaction = _object;
                                _object.Grab();

                                state = CursorState.Grab;
                                sprite.sprite = grabIcon;

                                joint.connectedBody = _object.GetComponent<Rigidbody2D>();

                                break;
                            }
                            else if (_transform.TryGetComponent<InteractObject>(out var _interaction))
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
                }
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
