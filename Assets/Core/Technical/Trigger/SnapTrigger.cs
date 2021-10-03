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
    public abstract class SnapTrigger : PhysicsTrigger
    {
        #region Global Members
        [Section("SnapTrigger")]
        [SerializeField] protected Vector2 snappingOffset = Vector2.zero;

        public bool HasSnappedObject { get; protected set; } = false; 
        protected Sequence snapSequence; 
        #endregion

        #region Methods
        public override void OnTrigger(PhysicsObject _object)
        {
            if (HasSnappedObject) return; 
            HasSnappedObject = true;
            _object.Rigidbody.velocity = Vector2.zero;
            _object.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll; 

            if (snapSequence != null) snapSequence.Complete(); 
            snapSequence = DOTween.Sequence();
            snapSequence.Join(_object.transform.DOMove((Vector2)transform.position + snappingOffset, .25f).SetEase(Ease.OutCirc));
            snapSequence.Join(_object.transform.DORotate(Vector3.zero, .25f).SetEase(Ease.OutCirc));
            snapSequence.Play(); 
        }

        public override void OnGrabbed(PhysicsObject _object) => HasSnappedObject = false;


        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere((Vector2)transform.position + snappingOffset, .1f); 
        }
        #endregion
    }
}
