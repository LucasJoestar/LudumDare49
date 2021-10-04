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
    public class FirePotionEffect : PotionEffect
    {
        #region Global Members
        [Section("FirePotionEffect")]
        [SerializeField, Range(.1f, 15.0f)] private float dropDelay = 2.0f;
        [SerializeField] private ParticleSystem fireEffect = null;
        [SerializeField] private AudioClip burnClip = null; 

        private Sequence dropSequence = null;
        #endregion

        #region Overriden Methods
        public override void OnCrashPotion()
        {
            // Smoke here + Fire
        }

        public override void OnDropPotion()
        {
            if (dropSequence.IsActive())
                dropSequence.Kill(); 
        }

        public override void OnForbiddenAction()
        {
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {
            if (dropSequence.IsActive())
                dropSequence.Kill();

            dropSequence = DOTween.Sequence();
            dropSequence.AppendInterval(dropDelay);
            dropSequence.OnComplete(DropPotionFromBurn); 
        }

        public override void OnShake()
        {
            // potion.Crush();
        }

        public override void OnStart()
        {
        }

        public override void OnTimeInterval()
        {
            // Destroy Potion from Potion Script
            // potion.Crush();
        }
        #endregion

        #region Methods
        private void DropPotionFromBurn()
        {
            potion.Drop();
            if (fireEffect != null)
                Instantiate(fireEffect, transform.position, Quaternion.identity);

            SoundManager.Instance.PlayAtPosition(burnClip, transform.position); 
        }
        #endregion
    }
}
