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
	public class Dispenser : SnapTrigger
    {
        #region Global Members
        [Section("Dispenser")]
        [SerializeField] private Transform pipeTransform = null;
        [Section("Action")]
        [SerializeField] private ActionPotion actionPotion = null;
        [SerializeField, ReadOnly] private Potion potion = null; 
        #endregion

        #region Methods
        private Sequence dispenserSequence = null;
        public void ActivateDispenser(Transform _leverTransform)
        {
            if (!HasSnappedObject) return;
            if (dispenserSequence != null) dispenserSequence.Kill(); 
            dispenserSequence = DOTween.Sequence();
            dispenserSequence.Append(_leverTransform.DOLocalRotate(new Vector3(0,0,-70), .5f));
            dispenserSequence.Append(pipeTransform.DOShakePosition(1.0f, .1f, 8));
            dispenserSequence.Append(pipeTransform.DOLocalMoveY(1.25f, .1f).OnComplete(ApplyPotionAction)); 
            dispenserSequence.Append(pipeTransform.DOLocalMoveY(1.0f, .1f));
            dispenserSequence.Append(_leverTransform.DOLocalRotate( Vector3.zero, .5f));
        }

        public override void OnTrigger(PhysicsObject _object)
        {
            if(_object is Potion)
            {
                base.OnTrigger(_object);
                potion = _object as Potion;
            }
        }

        public void ApplyPotionAction()
        {
            potion.ApplyAction(actionPotion); 
        }
        #endregion
    }
}
