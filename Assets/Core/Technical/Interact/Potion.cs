// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

namespace LudumDare49
{
	public class Potion : PhysicsObject
    {
        #region Global Members
        [Section("Potion")]

        [SerializeField, Required] protected Recipe recipe = null;
        [SerializeField, Required] protected Collider2D mixInCollider = null;
        [SerializeField, ReadOnly] protected int score = 0;

        public int Score => score;

        [Section("Recipe")]

        [SerializeField, ReadOnly] protected List<RecipeAction> remainingActions = new List<RecipeAction>();

        [Section("Audio")]

        [SerializeField, Required] protected AudioClip grabClip = null;
        [SerializeField, Required] protected AudioClip dropClip = null;

        [SerializeField, Required] protected AudioClip successClip = null;
        [SerializeField, Required] protected AudioClip failureClip = null;

        [Section("Feedback")]

        [SerializeField, Required] protected GameObject bubbleFX = null;

        [HelpBox("Magenta", MessageType.Info)]
        [SerializeField] protected Vector3 particleOffset = new Vector3();

        [Section("Potion Effect")]
        [SerializeField] private PotionEffect effect; 
        #endregion

        #region Behaviour
        public virtual void ApplyAction(PotionAction _action, bool _mixIngredient = false)
        {
            // Mix effect.
            if (_mixIngredient)
            {
                GameObject _particle = Instantiate(bubbleFX);
                _particle.transform.position = transform.position + grabPoint;
                _particle.transform.rotation = Quaternion.identity;
            }

            var _match = remainingActions.Find(a => a.Action == _action);
            if (_match != null)
            {
                // Recipe action.
                OnRecipeAction(_match, _action);
                SoundManager.Instance.PlayAtPosition(successClip, transform.position);
            }
            else
            {
                SoundManager.Instance.PlayAtPosition(failureClip, transform.position);
                _match = Array.Find(recipe.ForbiddenActions, a => a.Action == _action);

                if (_match != null)
                {
                    // Forbidden action.
                    OnForbiddenAction(_match, _action);
                    if (effect != null) effect.OnForbiddenAction(); 
                }
                else
                {
                    // Undesired action.
                    OnUndesiredAction(_action);
                }
            }
        }

        public override void Grab(PlayerCursor _cursor, HingeJoint2D _joint)
        {
            base.Grab(_cursor, _joint);
            mixInCollider.gameObject.SetActive(false);
            if(effect != null) effect.OnGrabPotion(_cursor);

            // Audio
            SoundManager.Instance.PlayAtPosition(grabClip, transform.position);
        }

        public override void Snap()
        {
            base.Snap();
            mixInCollider.gameObject.SetActive(true);

            SetLayer(ignoreLayer);
        }

        public override void Drop()
        {
            base.Drop();
            if (effect != null) effect.OnDropPotion();

            // Audio
            SoundManager.Instance.PlayAtPosition(dropClip, transform.position);
        }

        protected override void OnBrutalCollision(Vector3 _velocity)
        {
            base.OnBrutalCollision(_velocity);
            LoseObject(false);
        }

        public override void Shake()
        {
            base.Shake();
            if (effect != null) effect.OnShake();
        }

        // -----------------------

        protected virtual void OnRecipeAction(RecipeAction _recipeAction, PotionAction _potionAction)
        {
            // Update score.
            score += _recipeAction.Score;

            // Feedback.
            SoundManager.Instance.PlayAtPosition(_potionAction.AudioClip, rigidbody.position); 
            if (_potionAction.Particles)
            {
                ParticleSystem _particle = Instantiate(_potionAction.Particles);
                _particle.transform.position = transform.position + particleOffset;
                _particle.transform.rotation = Quaternion.identity;
            }
            
            // Update recipe.
            remainingActions.Remove(_recipeAction);
        }

        protected virtual void OnForbiddenAction(RecipeAction _recipeAction, PotionAction _potionAction)
        {
            // Update score.
            score += _recipeAction.Score;
        }

        protected virtual void OnUndesiredAction(PotionAction _action)
        {
            // Update score.
            score += recipe.UndesiredActionScore;
        }
        #endregion

        #region Mono Behaviour
        protected override void Awake()
        {
            base.Awake();

            remainingActions = new List<RecipeAction>(recipe.RecipeActions);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + particleOffset, .1f);
        }
        #endregion
    }
}
