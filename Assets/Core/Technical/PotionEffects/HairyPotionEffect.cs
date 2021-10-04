// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
    public class HairyPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("HairyPotionEffect")]

        [SerializeField, Required] private GameObject hairPrefab = null;
        [SerializeField, Required] private AudioClip hairGrow = null;
        [SerializeField, Required] private new Collider2D collider = null;

        private int spawnRate = 3;
        private int spawnCount = 0;
        #endregion

        #region Behaviour
        public override void OnCollideObject(Collider2D _collider, Vector2 _point)
        {
            spawnCount++;
            if (spawnCount < spawnRate)
                return;

            spawnCount = 0;

            GameObject _hair = Instantiate(hairPrefab);
            _hair.transform.position = _point;
            _hair.transform.rotation = Quaternion.identity;
            _hair.transform.SetParent(_collider.transform, true);

            SoundManager.Instance.PlayAtPosition(hairGrow, _point);
        }

        public override void OnCrashPotion()
        {
            
        }

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
