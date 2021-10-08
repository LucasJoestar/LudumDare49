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
    public class SeducingPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("SeducingPotionEffect")]
        [SerializeField, Required] private AudioSource source = null;
        [SerializeField] private PickupLine[] pickupLines = new PickupLine[] { };
        [SerializeField] private AudioClip[] pickupLinesClip = new AudioClip[] { }; 
        [SerializeField] private float instantiateForce = 10.0f;
        #endregion

        #region Overriden Methods
        public override void OnCrashPotion()
        {
        }

        public override void OnDropPotion()
        {
            source.DOKill();
            source.DOFade(0f, .5f).OnComplete(() => source.Stop());
        }

        public override void OnForbiddenAction()
        {
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {
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
            if (pickupLines.Length > 0)
            {
                int _count = Random.Range(0, pickupLines.Length);
                Rigidbody2D _bubble = Instantiate(pickupLines[_count], transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                _bubble.velocity = ((Vector2.up * Random.Range(-1.0f, 1.0f)) + (Vector2.left * Random.Range(-1.0f, 1.0f))) * instantiateForce;
            }
            if(pickupLinesClip.Length > 0)
            {
                int _count = Random.Range(0, pickupLinesClip.Length);
                SoundManager.Instance.PlayAtPosition(pickupLinesClip[_count], transform.position);
            }
            // ----- Repeat ----- // 
            timeSequence.Kill();
            timeSequence = DOTween.Sequence();
            timeSequence.AppendInterval(intervalTime);
            timeSequence.OnComplete(OnTimeInterval);
        }
        #endregion
    }
}
