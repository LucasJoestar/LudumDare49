// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class HeavyPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("HeavyPotionEffect")]

        [SerializeField, Range(1f, 25f)] private float speedCoef = 1f;
        [SerializeField, Range(0f, 5f)] private float speedCoefIncrement = 1f;
        [SerializeField, MinMax(1f, 25f)] private Vector2 speedCoefRange = new Vector2(1f, 25f);

        private PlayerCursor cursor = null;
        #endregion

        #region Behaviour
        public override void OnCollideObject(Collider2D _collider)
        {

        }

        public override void OnCrashPotion()
        {
            cursor.RemoveSpeedCoef(this);
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
