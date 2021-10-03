// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; 

namespace LudumDare49
{
    public class PackageBox : SnapTrigger
    {
        #region Global Members
        [Section("PackageBox")]
        [SerializeField, ReadOnly] private Potion potion = null;
        [SerializeField, ReadOnly] private Queue<PotionAction> pendingActions = new Queue<PotionAction>();
        [SerializeField] private Vector2 waypoint = Vector2.zero;
        [SerializeField] private Transform wayPointTransform;
        [SerializeField] private AnimationCurve movementCurve = new AnimationCurve(); 
        [SerializeField] private AnimationCurve rotationCurve = new AnimationCurve(); 
        #endregion

        #region Methods
        public override void OnTrigger(PhysicsObject _object)
        {
            HasSnappedObject = true;
            _object.Rigidbody.velocity = Vector2.zero;
            _object.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            wayPointTransform.localPosition = (waypoint + new Vector2(0,_object.GetComponent<SpriteRenderer>().bounds.extents.y)) / transform.localScale; 

            if (snapSequence != null) snapSequence.Complete();
            snapSequence = DOTween.Sequence();

            snapSequence.Join(_object.transform.DORotate(Vector3.zero, 0.01f).SetEase(movementCurve));

            snapSequence.Append(_object.transform.DOMove((Vector2)transform.position + waypoint, .8f).OnComplete(() => SetMaskInteraction(_object)));
            snapSequence.Append(wayPointTransform.DORotate(Vector3.back * 180, .15f).SetLoops(8, LoopType.Incremental).SetEase(rotationCurve)); 
            //snapSequence.Join(wayPointTransform.transform.DOScale(1.5f, .125f).SetLoops(4, LoopType.Yoyo));

            snapSequence.Append(wayPointTransform.DOMove((Vector2)transform.position + snappingOffset, .25f).OnComplete(() => SetObject(_object)));
            snapSequence.Play();
        }

        private void SetMaskInteraction(PhysicsObject _object)
        {
            _object.transform.SetParent(wayPointTransform); 
            _object.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask; 
        }

        private void SetObject(PhysicsObject _object)
        {
            _object.transform.SetParent(transform);

            if (_object is Potion)
            {
                potion = (Potion)_object;
                ApplyPendingActions();
            }
            else if (_object is Ingredient)
            {
                pendingActions.Enqueue((_object as Ingredient).Action);
                if (potion != null)
                {
                    ApplyPendingActions();
                }
            }
        }

        private void ApplyPendingActions()
        {
            if (potion == null) return;
            PotionAction _action = null; 
            while (pendingActions.Count > 0)
            {
                _action = pendingActions.Dequeue();
                potion.ApplyAction(_action); 
            }
        }

        public void StampPackage(PotionAction _stampAction)
        {
            pendingActions.Enqueue(_stampAction);
            ApplyPendingActions(); 
        }
        #endregion

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawSphere(transform.position + (Vector3)waypoint, .1f); 
        }
    }
}
