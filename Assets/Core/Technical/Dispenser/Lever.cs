// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;
using DG.Tweening;
using System;

namespace LudumDare49
{
	public class Lever : MonoBehaviour, IInteractObject
    {
        #region Global Members
        public event Action OnLeverPull = null; 
        [Section("Lever")]
        private Sequence leverSequence = null;
        private Collider2D leverCollider; 
        #endregion

        #region Methods
        void IInteractObject.Interact()
        {
            if (leverSequence.IsActive())
                leverSequence.Kill(); 

            leverCollider.enabled = false; 
            leverSequence = DOTween.Sequence();
            leverSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -70), .5f).OnComplete(OnLeverPull.Invoke));
        }

        public void ResetLever()
        {
            if (leverSequence.IsActive())
                leverSequence.Kill();
            leverSequence = DOTween.Sequence();
            leverSequence.Append(transform.DOLocalRotate(Vector3.zero, .5f)).OnComplete(() => leverCollider.enabled = true );  ;
        }

        private void Awake()
        {
            leverCollider = GetComponent<Collider2D>(); 
        }
        #endregion
    }
}
