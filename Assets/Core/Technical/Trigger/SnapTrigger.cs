// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using DG.Tweening; 

namespace LudumDare49
{
    public class SnapTrigger : PhysicsTrigger
    {
        #region Global Members
        [Section("SnapTrigger")]
        [SerializeField] protected Vector2 snappingOffset = Vector2.zero;
        [SerializeField] protected new Collider2D collider = null;

        [SerializeField, ReadOnly()] protected bool hasSnappedObject = false; 
        protected Sequence snapSequence; 
        #endregion

        #region Methods
        public override void OnTrigger(PhysicsObject _object)
        {
            if (!hasSnappedObject)
            {
                hasSnappedObject = true;
                _object.Snap();

                if (snapSequence.IsActive()) snapSequence.Complete(); 
                snapSequence = DOTween.Sequence();
                snapSequence.Join(_object.transform.DOMove((Vector2)transform.position + snappingOffset, .25f).SetEase(Ease.OutCirc));
                snapSequence.Join(_object.transform.DORotate(Vector3.zero, .25f).SetEase(Ease.OutCirc));
                snapSequence.Play();

                collider.enabled = false;
            }
        }

        public override void OnGrabbed(PhysicsObject _object)
        {
            hasSnappedObject = false;
            collider.enabled = true;
        }


        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere((Vector2)transform.position + snappingOffset, .1f); 
        }
        #endregion
    }
}
