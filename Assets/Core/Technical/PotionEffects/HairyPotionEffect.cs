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
    public class HairyPotionEffect : PotionEffect
    {
        #region Global Members
        [Section("HairyPotionEffect")]

        [SerializeField, Required] private GameObject hairPrefab = null;
        [SerializeField, Required] private AudioClip hairGrow = null;
        [SerializeField, Required] private AudioSource source = null;

        private int spawnRate = 2;
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

            SoundManager.Instance.PlayAtPosition(hairGrow, _point);
        }

        public override bool OnCrashPotion() => false;

        public override void OnDropPotion()
        {
            source.DOKill();
            source.DOFade(0f, .5f).OnComplete(() => source.Stop());
        }

        public override void OnForbiddenAction()
        {
            spawnRate = Mathf.Max(1, spawnRate - 1);
        }

        public override void OnGrabPotion(PlayerCursor _cursor)
        {
            source.DOKill();
            source.DOFade(1f, .5f);
            source.Play();
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
