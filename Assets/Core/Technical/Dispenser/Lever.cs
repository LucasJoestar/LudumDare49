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
        [SerializeField, Range(.1f, 5.0f)] private float returnTime = 1.0f;
        private Sequence leverSequence = null; 
        #endregion

        #region Methods
        void IInteractObject.Interact()
        {
            if (leverSequence.IsActive())
                leverSequence.Kill();
            leverSequence = DOTween.Sequence();
            leverSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -70), .5f).OnComplete(OnLeverPull.Invoke));
            leverSequence.AppendInterval(returnTime);
            leverSequence.Append(transform.DOLocalRotate(Vector3.zero, .5f));
        }
        #endregion
    }
}
