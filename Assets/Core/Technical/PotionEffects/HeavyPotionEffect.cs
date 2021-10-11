// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using DG.Tweening;
using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class HeavyPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("HeavyPotionEffect")]

        [SerializeField, Range(.1f, 15.0f)] private float shakeDuration = 1.0f;

        [Space(5f)]

        [SerializeField, Range(1f, 25f)] private float speedCoef = 1f;
        [SerializeField, Range(0f, 5f)] private float speedCoefIncrement = 1f;
        [SerializeField, MinMax(1f, 25f)] private Vector2 speedCoefRange = new Vector2(1f, 25f);

        private PlayerCursor cursor = null;
        #endregion

        #region Behaviour
        public override void OnCollideObject(Collider2D _collider, Vector2 _point)
        {

        }

        public override bool OnCrashPotion()
        {
            cursor.RemoveSpeedCoef(this);
            Camera.main.transform.DOShakePosition(shakeDuration, 1, 15);
            return false; 
        }

        public override void OnDropPotion()
        {
            cursor.RemoveSpeedCoef(this);
        }

        public override void OnForbiddenAction()
        {
            speedCoef = Mathf.Clamp(speedCoef + speedCoefIncrement, speedCoefRange.x, speedCoefRange.y);
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {
            cursor.SetSpeedCoef(this, 1f / speedCoef);
        }

        public override void OnRecipeAction()
        {
            speedCoef = Mathf.Clamp(speedCoef - speedCoefIncrement, speedCoefRange.x, speedCoefRange.y);
        }

        public override void OnShake()
        {
            Camera.main.transform.DOShakePosition(shakeDuration, 1, 15);
        }

        public override void OnStart()
        {
            cursor = FindObjectOfType<PlayerCursor>();
        }

        public override void OnTimeInterval()
        {

        }
        #endregion
    }
}
