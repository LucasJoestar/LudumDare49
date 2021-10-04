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
	public class PackageDispenser : MonoBehaviour
    {
        #region Global Members
        [Section("PackageDispenser")]
        [SerializeField] private PackageBox packagePrefab;
        [SerializeField] private ConveyorBelt conveyorBelt;
        [SerializeField] private Lever lever;
        [Section("Animation")]
        [SerializeField, Range(.5f, 2.0f)] private float waitingTime = 1.0f;
        [SerializeField] private Vector2 offsetPosition = Vector2.zero; 
        private PackageBox currentBox = null;
        private bool canValidate = false; 
        #endregion

        #region Methods
        private Sequence movementSequence; 
        public void DispensePackage()
        {
            if (movementSequence.IsActive())
                movementSequence.Kill();

            movementSequence = DOTween.Sequence();
            currentBox = Instantiate(packagePrefab, transform.position, Quaternion.identity);
            movementSequence.AppendInterval(waitingTime);
            movementSequence.Append(transform.DOShakePosition(1.0f, .1f, 8));
            movementSequence.Append(currentBox.transform.DOMove((Vector2)transform.position + offsetPosition, .15f));
            movementSequence.Append(currentBox.transform.DOScaleY(.4f, .1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad));
            movementSequence.OnComplete(() => canValidate = true);
        }

        public void ValidatePackage()
        {
            // Return lever at original rotation
            lever.ResetLever();
            if (canValidate)
            {
                canValidate = false; 
                currentBox.ClosePackage(); 
                conveyorBelt.Roll(currentBox.transform);
            }
        }

        public void OnPackageSent()
        {
            // UPDATE SCORE HERE
            Destroy(currentBox.gameObject);
            DispensePackage();
            // Return lever at original rotation
            //lever.ResetLever();
        }

        private void Start()
        {
            conveyorBelt.OnEndRoll += OnPackageSent;
            lever.OnLeverPull += ValidatePackage;
            DispensePackage(); 
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere((Vector2)transform.position + offsetPosition, .1f);
        }
        #endregion 
    }
}
