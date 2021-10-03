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
        [SerializeField] private Vector2 snappingOffset = Vector2.zero;

        public bool HasSnappedObject { get; private set; }
        private Sequence snapSequence; 
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere((Vector2)transform.position + snappingOffset, .1f); 
        }
        #endregion
    }
}
