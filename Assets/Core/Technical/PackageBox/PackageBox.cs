// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering; 

namespace LudumDare49
{
    public class PackageBox : PhysicsTrigger
    {
        #region Global Members
        [Section("PackageBox")]
        [SerializeField] private new Collider2D collider = null; 
        [SerializeField, ReadOnly] private Potion potion = null;
        [SerializeField, ReadOnly] private Queue<PotionAction> pendingActions = new Queue<PotionAction>();

        [SerializeField] private Vector2 snappingOffset = Vector2.zero; 
        [SerializeField] private Vector2 potionOffset = Vector2.zero;
        [SerializeField, Range(-90, 90)] private float potionRotation = -14.0f;
        [SerializeField, Range(.1f, 2.0f)] private float rotationDuration = .1f;
        [SerializeField] private Vector2[] ingredientOffset = new Vector2[] { }; 

        private static readonly PhysicsObject[] pendingObject = new PhysicsObject[2];
        [SerializeField] private SortingGroup sortingGroup = null; 

        [Section("Sending settings")]
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private AudioClip closingClip;

        private Sequence snapSequence = null; 
        #endregion

        #region Methods
        public override void OnTrigger(PhysicsObject _object)
        {
            if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 1;

            if(_object is Potion _potion)
            {
                if (potion != null) return; 
            }
            else if(_object is Ingredient _ingredient)
            {
                if (pendingObject[0] != null && pendingObject[1] != null)
                    return; 
            }

            _object.Snap();
            _object.GetComponent<Collider2D>().enabled = false; 

            if (snapSequence.IsActive()) snapSequence.Complete();
            snapSequence = DOTween.Sequence();
            snapSequence.Join(_object.transform.DOMove((Vector2)transform.position + snappingOffset, .25f).SetEase(Ease.OutCirc));
            snapSequence.Join(_object.transform.DORotate(Vector3.zero, .25f).SetEase(Ease.OutCirc));
            snapSequence.Play();

            collider.enabled = false;

            snapSequence.OnComplete(() => SetObject(_object));
        }

        private void SetObject(PhysicsObject _object)
        {
            _object.GetComponentInChildren<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            _object.transform.SetParent(transform);

            snapSequence.Kill();
            snapSequence = DOTween.Sequence();
            snapSequence.AppendInterval(.1f);

            Vector2 _offsetPosition = Vector2.zero; 
            if (_object is Potion _potion )
            {
                snapSequence.Append(_object.transform.DORotate(Vector3.forward * potionRotation, rotationDuration)); 
                _offsetPosition = potionOffset; 
                potion = _potion;
            }
            else if (_object is Ingredient _ingredient)
            {
                snapSequence.Append(_object.transform.DORotate(Vector3.zero, rotationDuration)); 
                if (pendingObject[0] == null)
                {
                    pendingObject[0] = _object;
                    _offsetPosition = ingredientOffset[0]; 
                }
                else
                {
                    pendingObject[1] = _object;
                    if(pendingObject[0].TryGetComponent(out Flame _f) && _object.TryGetComponent(out Flame _f2))
                    {
                        _f.MakeHorny(); 
                        _f2.MakeHorny(); 
                    }
                    _offsetPosition = ingredientOffset[1]; 
                }
                
                pendingActions.Enqueue(_ingredient.Action);
            }
            snapSequence.Append(_object.transform.DOMove((Vector2)transform.position + _offsetPosition, rotationDuration));
            snapSequence.OnComplete(ApplyPendingActions);

            collider.enabled = true; 
        }

        private void ApplyPendingActions()
        {
            if (potion == null) return;
            PotionAction _action = null; 
            while (pendingActions.Count > 0)
            {
                _action = pendingActions.Dequeue();
                potion.ApplyAction(_action); 
            }
        }

        public void StampPackage(PotionAction _stampAction)
        {
            pendingActions.Enqueue(_stampAction);
            ApplyPendingActions(); 
        }

        public void ClosePackage()
        {
            GetComponentInChildren<SpriteRenderer>().sprite = closedSprite;
            if(potion != null) potion.GetComponentInChildren<SpriteRenderer>().enabled = false;
            for (int i = 0; i < pendingObject.Length; i++)
            {
                if (pendingObject[i] == null) continue; 
                pendingObject[i].GetComponentInChildren<SpriteRenderer>().enabled = false; 
            }
            SoundManager.Instance.PlayAtPosition(closingClip, transform.position);
            SendPackage(); 
        }

        #region Score Methods
        public void SendPackage()
        {
            if(potion != null)
            {
                ScoreManager.Instance.IncrementScore(potion.Score);
            }
        }
        #endregion

        #endregion

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + (Vector3)snappingOffset, .1f);
            Gizmos.color = Color.red; 
            Gizmos.DrawSphere(transform.position + (Vector3)potionOffset, .1f);
            Gizmos.color = Color.green;
            for (int i = 0; i < ingredientOffset.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)ingredientOffset[i], .1f);
            }
        }
    }
}
