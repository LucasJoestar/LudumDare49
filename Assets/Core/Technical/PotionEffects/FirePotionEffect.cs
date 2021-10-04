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
        [SerializeField] private GameObject fireEffect = null;
        [SerializeField] private AudioClip burnClip = null;
        [Section("Crashing Effect")]
        [SerializeField] private Vector2Int minMaxFireEffect = new Vector2Int(1, 2);
        [SerializeField, Range(.1f, 15.0f)] private float shakeDuration = 1.0f;
        [SerializeField, Range(1, 10)] private float flameForce = 2.0f;
        [Section("Flame Disapearing")]
        [SerializeField, Range(1, 10)] private float waitingTime = 1.0f; 
        [SerializeField, Range(1, 10)] private float shrinkingDuration = 1.0f; 

        private PlayerCursor cursor = null; 
        private Sequence dropSequence = null;
        #endregion

        #region Overriden Methods
        public override void OnCrashPotion()
        {
            // Smoke here + Fire
            Sequence _crashSequence = DOTween.Sequence();
            _crashSequence.Append(Camera.main.transform.DOShakePosition(shakeDuration, 1, 15)).OnComplete(() => Destroy(potion.gameObject));
            if(fireEffect != null)
            {
                int _count = Random.Range(minMaxFireEffect.x, minMaxFireEffect.y + 1);
                Rigidbody2D _flame; 
                for (int i = 0; i < _count; i++)
                {
                    _flame = Instantiate(fireEffect, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                    _flame.velocity = (Vector2.up + (Vector2.left * Random.Range(-1.0f, 1.0f))) * flameForce;
                }
            }
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
            cursor = _cursor; 

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
            cursor.Drop();
            // potion.Drop();
            if (fireEffect != null)
            {
                Rigidbody2D _flame = Instantiate(fireEffect, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                _flame.velocity = (Vector2.up + (Vector2.left * Random.Range(-1.0f, 1.0f))) * flameForce;
                Sequence _s = DOTween.Sequence();
                _s.AppendInterval(waitingTime);
                _s.Append(_flame.transform.DOScale(0, shrinkingDuration));
                _s.OnComplete(() => Destroy(_flame.gameObject));
            }

            SoundManager.Instance.PlayAtPosition(burnClip, transform.position); 
        }
        #endregion
    }
}
