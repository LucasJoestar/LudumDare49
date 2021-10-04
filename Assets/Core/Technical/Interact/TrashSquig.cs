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
	public class TrashSquig : PhysicsTrigger
    {
        #region State
        public enum SquigState
        {
            Hidden,
            Wait,
            Target,
            Eat
        }
        #endregion

        #region Global Members
        [Section("Trash Squig")]

        [SerializeField, Required] private Collider2D generalCollider = null;
        [SerializeField, Required] private Collider2D eatCollider = null;
        [SerializeField, Required] private SpriteRenderer sprite = null;
        [SerializeField, Required] private new AudioSource audio = null;
        [SerializeField, Required] private AudioClip eatClip = null;

        [Space(5f)]

        [SerializeField, Required] private Sprite waitSprite = null;
        [SerializeField, Required] private Sprite eatSprite = null;
        [SerializeField, Required] private Sprite happySprite = null;

        [Space(5f)]

        [SerializeField, Required] private PlayerCursor cursor = null;

        [Section("Settings")]

        [SerializeField] private Vector3 hiddenPosition = Vector3.zero;
        [SerializeField] private Vector3 waitPosition = Vector3.zero;

        [Space(5f)]

        [SerializeField, Range(0f, 2f)] private float hideDuration = .5f;
        [SerializeField, Range(0f, 2f)] private float showDuration = .5f;

        [SerializeField] private Ease hideEase = Ease.Linear;
        [SerializeField] private Ease showEase = Ease.Linear;

        [Section("State")]

        [SerializeField, ReadOnly] private SquigState state = SquigState.Hidden;
        private PhysicsObject target = null;
        #endregion

        #region Behaviour
        private Sequence sequence = null;

        // -----------------------

        public override void OnTrigger(PhysicsObject _object)
        {
            target = _object;
            state = SquigState.Target;
        }

        // -----------------------

        private void Update()
        {
            switch (state)
            {
                case SquigState.Hidden:
                    if ((cursor.State == PlayerCursor.CursorState.Grab) && generalCollider.bounds.Contains(cursor.WorldTransform.position))
                    {
                        // Kill sequence.
                        if (sequence.IsActive())
                            sequence.Kill(false);

                        // Play anim.
                        sequence = DOTween.Sequence();
                        sequence.Join(sprite.transform.DOMoveY(waitPosition.y, showDuration).SetEase(showEase));

                        // Audio.
                        audio.Play();
                        sequence.Join(audio.DOFade(1f, .2f));

                        sprite.sprite = waitSprite;

                        // Set state.
                        state = SquigState.Wait;
                    }
                    break;

                case SquigState.Wait:
                    if ((cursor.State != PlayerCursor.CursorState.Grab) || !generalCollider.bounds.Contains(cursor.WorldTransform.position))
                    {
                        // Hide.
                        Hide(waitSprite);
                    }
                    break;

                case SquigState.Target:
                    if (target.IsGrabbed)
                    {
                        // Wait.
                        state = SquigState.Wait;
                    }
                    else if (eatCollider.Distance(target.Collider).isOverlapped)
                    {
                        // Eat.
                        sprite.sprite = eatSprite;
                        target.Eat();
                        target = null;

                        state = SquigState.Eat;

                        // Kill sequence.
                        if (sequence.IsActive())
                            sequence.Kill(false);

                        // Play anim.
                        sequence = DOTween.Sequence();
                        sequence.AppendInterval(.5f);
                        sequence.OnComplete(() => Hide(happySprite));

                        // Audio.
                        audio.Stop();
                        audio.volume = 0f;
                        SoundManager.Instance.PlayAtPosition(eatClip, sprite.transform.position);
                    }
                    else if (!generalCollider.Distance(target.Collider).isOverlapped)
                    {
                        // Hide.
                        Hide(waitSprite);
                    }
                    break;

                case SquigState.Eat:
                    // Wait sequence.
                    break;

                default:
                    break;
            }

            // ----- Local Methods ----- \\

            void Hide(Sprite _sprite)
            {
                // Kill sequence.
                if (sequence.IsActive())
                    sequence.Kill(false);

                // Play anim.
                sequence = DOTween.Sequence();
                sequence.Join(sprite.transform.DOMoveY(hiddenPosition.y, hideDuration).SetEase(hideEase));
                sequence.Join(audio.DOFade(0f, .2f).OnComplete(audio.Stop));

                sprite.sprite = _sprite;

                // Set state.
                state = SquigState.Hidden;
            }
        }

        private void Start()
        {
            audio.volume = 0f;
        }
        #endregion
    }
}
