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
    public class FreezingPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("FreezingPotionEffect")]
        [SerializeField, Range(.1f, 1.0f)] private float freezingSpeedCoeff = .75f;
        [SerializeField, Required] private AudioSource source = null;
        [SerializeField] private SpriteRenderer freezingEffect = null;
        [SerializeField, Range(.1f, 2.0f)] private float transitionDuration = .5f;
        private Sequence transitionSequence = null;
        private PlayerCursor cursor = null; 
        #endregion

        #region Methods
        public override void OnCrashPotion()
        {
            // Ne Behaviour here
        }

        public override void OnDropPotion()
        {
            if (transitionSequence.IsActive())
                transitionSequence.Kill();
            transitionSequence = DOTween.Sequence();
            transitionSequence.Join(freezingEffect.DOFade(0.0f, transitionDuration));
            // reset cursor speed
            cursor.RemoveSpeedCoef(this);

            source.DOKill();
            source.DOFade(0f, .5f).OnComplete(() => source.Stop());
        }

        public override void OnForbiddenAction()
        {
            // 
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {
            if (transitionSequence.IsActive())
                transitionSequence.Kill();
            transitionSequence = DOTween.Sequence();
            transitionSequence.Join(freezingEffect.DOFade(1.0f, transitionDuration));
            //Change Cursor Speed
            cursor = _cursor;
           _cursor.SetSpeedCoef(this, freezingSpeedCoeff);

            source.DOKill();
            source.DOFade(1f, .5f);
            source.Play();
        }

        public override void OnCollideObject(Collider2D _collider, Vector2 _point)
        {

        }

        public override void OnRecipeAction()
        {

        }

        public override void OnShake()
        {
            
        }

        public override void OnStart()
        {

        }

        public override void OnTimeInterval()
        {
            // No Behaviour Here
        }

        protected override void Start()
        {
            base.Start();
            freezingEffect = Instantiate(freezingEffect, Vector3.zero, Quaternion.identity); 
        }
        #endregion
    }
}
