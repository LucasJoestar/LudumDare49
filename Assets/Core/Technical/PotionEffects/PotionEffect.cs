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
	public abstract class PotionEffect : MonoBehaviour
    {
        #region Global Members
        [Section("PotionEffect")]
        [SerializeField] protected bool useTimeInterval = false;
        [SerializeField, Range(1.0f, 25.0f)] protected float intervalTime = 10.0f;
        protected Sequence timeSequence = null;
        protected Potion potion = null; 
        #endregion

        #region Methods
        public abstract void OnGrabPotion(PlayerCursor _cursor);
        public abstract void OnDropPotion();
        public abstract void OnCrashPotion();
        public abstract void OnTimeInterval();
        public abstract void OnForbiddenAction();
        public abstract void OnShake();

        protected virtual void Start()
        {
            potion = GetComponent<Potion>();
            if(useTimeInterval)
            {
                timeSequence = DOTween.Sequence();
                timeSequence.AppendInterval(intervalTime); 
                timeSequence.OnComplete(OnTimeInterval);
            }
        }
        #endregion
    }
}
