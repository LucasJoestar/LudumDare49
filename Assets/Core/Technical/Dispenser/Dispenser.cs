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
        [SerializeField] private Lever pipeLever = null; 
        [Section("Action")]
        [SerializeField] private PotionAction actionPotion = null;
        [SerializeField, ReadOnly] private Potion potion = null;
        [Section("Feedback")]
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private AudioClip audioClip;
        #endregion

        #region Methods
        private Sequence dispenserSequence = null;
        public void ActivateDispenser()
        {
            if (!HasSnappedObject)
            {
                // Return lever at original rotation
                pipeLever.ResetLever();
                return;
            }
            if (dispenserSequence.IsActive()) dispenserSequence.Kill(); 
            dispenserSequence = DOTween.Sequence();
            dispenserSequence.Append(pipeTransform.DOShakePosition(1.0f, .1f, 8));
            dispenserSequence.Append(pipeTransform.DOLocalMoveY(1.05f, .1f).OnComplete(ApplyPotionAction)); 
            dispenserSequence.Append(pipeTransform.DOLocalMoveY(1.0f, .1f));
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
        }

        private void Start()
        {
            pipeLever.OnLeverPull += ActivateDispenser;
        }
        #endregion
    }
}
