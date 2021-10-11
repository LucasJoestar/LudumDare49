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
	public class GameClock : MonoBehaviour
    {
        #region Global Members
        [Section("GameClock")]
        [SerializeField] private Transform handTransform;
        [SerializeField, Range(10, 600)] private float gameTime = 10;
        [SerializeField] private AnimationCurve easeCurve = new AnimationCurve();
        [SerializeField] private AudioClip clip; 
        private Sequence clockSequence; 
        #endregion

        #region Methods
        private void Start()
        {
            clockSequence = DOTween.Sequence();
            clockSequence.Append(handTransform.DORotate(Vector3.back * 180, gameTime/2).SetLoops(2, LoopType.Incremental).SetEase(easeCurve));
            clockSequence.OnComplete(() => SoundManager.Instance.PlayAtPosition(clip, transform.position)); 
        }
        #endregion 
    }
}
