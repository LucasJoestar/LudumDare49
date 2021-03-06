// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using DG.Tweening;
using System;
using UnityEngine;

namespace LudumDare49
{
	public class ConveyorBelt : MonoBehaviour
    {
        public event Action OnEndRoll = null;

        #region Global Members
        [Section("Conveyor Belt")]

        [SerializeField, Required] private new AudioSource audio = null;
        [SerializeField, Required] private SpriteRenderer link = null;

        [Space(5f)]

        [SerializeField, Required] private Transform rightBelt = null;
        [SerializeField, Required] private Transform leftBelt = null;
        [SerializeField] private Transform[] wheels = new Transform[] { };

        [Section("Settings")]

        [SerializeField] private float loopTime = .5f;
        [SerializeField] private Ease beltEase = Ease.Linear;
        [SerializeField] private Ease wheelEase = Ease.Linear;
        #endregion

        #region Behaviour
        private Vector3 rightBeltOrigin = Vector3.zero;
        private Vector3 leftBeltOrigin = Vector3.zero;
        private Sequence sequence = null;

        // -----------------------

        public void Roll(Transform _article, int _loop = 5)
        {
            // Belt sequence.
            if (sequence.IsActive())
                sequence.Kill(true);

            sequence = DOTween.Sequence();
            sequence.Join(rightBelt.DOMoveX(rightBelt.transform.position.x + link.bounds.size.x, loopTime).SetEase(beltEase));
            sequence.Join(leftBelt.DOMoveX(leftBelt.transform.position.x - link.bounds.size.x, loopTime).SetEase(beltEase));

            foreach (var _wheel in wheels)
            {
                sequence.Join(_wheel.DORotate(new Vector3(0f, 0f, _wheel.rotation.eulerAngles.z + 360f), loopTime, RotateMode.FastBeyond360).SetEase(wheelEase));
            }

            sequence.SetLoops(_loop, LoopType.Restart);

            // Article sequence.
            Sequence _articleSequence = DOTween.Sequence();

            _articleSequence.Join(_article.DOMoveX(_article.position.x + link.bounds.size.x, loopTime).SetEase(beltEase));
            _articleSequence.SetLoops(_loop, LoopType.Incremental);

            // Audio.
            audio.Play();
            audio.DOFade(1f, .2f);

            sequence.OnComplete(() =>
            {
                audio.DOFade(0f, .2f).OnComplete(audio.Stop);
                OnEndRoll?.Invoke();

                rightBelt.position = rightBeltOrigin;
                leftBelt.position = leftBeltOrigin;
            });
        }

        private void Start()
        {
            audio.volume = 0f;
            rightBeltOrigin = rightBelt.position;
            leftBeltOrigin = leftBelt.position;
        }
        #endregion
    }
}
