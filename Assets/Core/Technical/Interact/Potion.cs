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

        [Section("Feedback")]

        [HelpBox("Magenta", MessageType.Info)]
        [SerializeField] protected Vector3 particleOffset = new Vector3();
        #endregion

        #region Behaviour
        public virtual void ApplyAction(PotionAction _action)
        {
            var _match = remainingActions.Find(a => a.Action == _action);
            if (_match != null)
            {
                // Recipe action.
                OnRecipeAction(_match, _action);                
            }
            else
            {
                _match = Array.Find(recipe.ForbiddenActions, a => a.Action == _action);
                if (_match != null)
                {
                    // Forbidden action.
                    OnForbiddenAction(_match, _action);
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
        }

        public override void Snap()
        {
            base.Snap();
            mixInCollider.gameObject.SetActive(true);

            SetLayer(ignoreLayer);
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
