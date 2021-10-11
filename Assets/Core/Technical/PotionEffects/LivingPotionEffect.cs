// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class LivingPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("LivingPotionEffect")]

        [SerializeField, MinMax(1f, 60f)] private Vector2 lifeDuration = new Vector2();
        [SerializeField, Range(0f, 50f)] private float lifeCoef = 2;

        private int spawnRate = 2;
        private int spawnCount = 0;
        #endregion

        #region Behaviour
        public override void OnCollideObject(Collider2D _collider, Vector2 _point)
        {
            if (_collider.TryGetComponent<LivingReceptable>(out LivingReceptable _receptacle))
            {
                spawnCount++;
                if (spawnCount < spawnRate)
                    return;

                spawnCount = 0;

                float _duration = Random.Range(lifeDuration.x, lifeDuration.y) * lifeCoef * spawnRate;
                _receptacle.Generate(_duration);
            }
        }

        public override bool OnCrashPotion() => false; 

        public override void OnDropPotion()
        {

        }

        public override void OnForbiddenAction()
        {
            spawnRate = Mathf.Max(1, spawnRate - 1);
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {

        }

        public override void OnRecipeAction()
        {
            spawnRate++;
        }

        public override void OnShake()
        {
            spawnRate = Mathf.Max(1, spawnRate - 1);
        }

        public override void OnStart()
        {

        }

        public override void OnTimeInterval()
        {

        }
        #endregion
    }
}
