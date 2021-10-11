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
        // Return true if the potion has to be destroyed outside of the method OnCrashPotion.
        public abstract bool OnCrashPotion();
        public abstract void OnCollideObject(Collider2D _collider, Vector2 _point);
        public abstract void OnTimeInterval();
        public abstract void OnRecipeAction();
        public abstract void OnForbiddenAction();
        public abstract void OnShake();
        public abstract void OnStart();
        
        protected virtual void Start()
        {
            potion = GetComponent<Potion>();
            if(useTimeInterval)
            {
                timeSequence = DOTween.Sequence();
                timeSequence.AppendInterval(intervalTime); 
                timeSequence.OnComplete(OnTimeInterval);
            }
            OnStart();
        }
        #endregion
    }
}
