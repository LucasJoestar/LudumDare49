// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Rendering; 

namespace LudumDare49
{
    public class Stamp : MonoBehaviour, IGrabbable
    {
        #region Global Members
        [Section("Stamp")]
        [SerializeField, ReadOnly] private Transform handTransform = null;
        [SerializeField, Range(1.0f,10.0f)] private float returningSpeed = 2.0f;
        [Section("Stamp Rendering")]
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private Sprite normalSprite = null; 
        [SerializeField] private Sprite grabbedSprite = null;
        [Section("package Detection")]
        [SerializeField, HelpBox("Layer mask of the package object", MessageType.Info)] private LayerMask packageLayer;
        [SerializeField] private SortingLayer backgroundSortingLayer;
        [SerializeField] private SortingLayer BoxSortingLayer;
        [SerializeField] private PotionAction action; 

        private bool isGrabbed = false;
        private Sequence returningSequence;
        private Vector2 startPosition;
        #endregion

        #region Methods

        public void Drop()
        {
            isGrabbed = false;
            transform.rotation = Quaternion.identity; 
            spriteRenderer.sprite = normalSprite; 
            // Reset Stamp Position
            if (returningSequence != null) returningSequence.Kill();
            returningSequence = DOTween.Sequence();
            float _duration = Vector2.Distance(transform.position, startPosition) / returningSpeed; 
            returningSequence.Join(transform.DOMove(startPosition, _duration));
            returningSequence.Play(); 
        }

        public void Grab(HingeJoint2D _joint)
        {
            isGrabbed = true;
            handTransform = _joint.transform;
            spriteRenderer.sprite = grabbedSprite;
        }


        [SerializeField] private SpriteRenderer[] marks = new SpriteRenderer[5];
        private Sequence DestroyLastStampMarkSequence = null;
        private Sequence StampSequence = null; 
        private void ApplyStamp(bool _isOnPackage)
        {
            
            if (marks[marks.Length - 1] != null)
            {
                if (DestroyLastStampMarkSequence != null)
                {
                    DestroyLastStampMarkSequence.Complete();
                }
                DestroyLastStampMarkSequence = DOTween.Sequence();
                SpriteRenderer _renderer = marks[marks.Length - 1].GetComponent<SpriteRenderer>();
                DestroyLastStampMarkSequence.Join(_renderer.DOFade(0, 1.0f)).OnComplete(() => marks[0] = _renderer); 
            }
            

            if (StampSequence != null) StampSequence.Kill();
            StampSequence = DOTween.Sequence(); 
            StampSequence.Append(transform.DORotate(Vector3.forward * Random.value * 45 * (Random.value > .5f ? -1 : 1), .05f)) ;
            StampSequence.Append(transform.DOScale(.5f, .1f).OnComplete(() => InstanciateStampMark(_isOnPackage))); 
            StampSequence.Append(transform.DOScale(1.0f, .5f));
        }

        private void InstanciateStampMark(bool _isOnPackage)
        {
            if(_isOnPackage)
            {
                /*
                PotionPackage _package; 
                if(hits[0].transform.TryGetComponent<PotionPackage>(out _package))
                {
                    marks[0].GetComponent<SortingGroup>().sortingLayerID = BoxSortingLayer.id; 
                    _package.ApplyAction(action);
                }
                */
            }
            marks[0].GetComponent<SortingGroup>().sortingLayerID = backgroundSortingLayer.id;
            marks[0].transform.position = transform.position;
            marks[0].transform.rotation = transform.rotation;
            marks[0].color = Color.white;

            for (int i = marks.Length; i-- > 1;)
            {
                marks[i] = marks[i - 1];
            }
        }

        private static RaycastHit2D[] hits = new RaycastHit2D[1]; 
        private void Update()
        {
            // Update Stamp position if is held
            if(isGrabbed)
            {
                transform.position = handTransform.position;
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    // Get Box object.
                    int _amount = Physics2D.RaycastNonAlloc(transform.position, Vector2.zero, hits, packageLayer);

                    ApplyStamp(_amount > 0); 
                }
            }
        }

        private void Start()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        #endregion
    }
}
