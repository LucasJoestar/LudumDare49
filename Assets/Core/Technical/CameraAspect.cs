// ===== Ludum Dare #49 - https://github.com/LucasJoestar/LudumDare49 ===== //
//
// Notes:
//
// ======================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace LudumDare49
{
	public class CameraAspect : MonoBehaviour
    {
        /// <summary>
        /// Target aspect of the game.
        /// </summary>
        public const float TargetAspect = 16f / 9f;

        #region Global Members
        [Section("PhysicsObject")]

        [SerializeField] private new Camera camera = null;
        private float lastAspect = 1f;

        // -----------------------

        private void Update()
        {
            float _currentAspect = (float)Screen.width / Screen.height;
            if (_currentAspect == lastAspect)
                return;

            lastAspect = _currentAspect;
            float _scaledHeight = _currentAspect / TargetAspect;

            // Set new camera aspect.
            if (_scaledHeight < 1f)
            {
                Rect _rect = new Rect()
                {
                    x = 0f,
                    y = (1f - _scaledHeight) / 2f,
                    width = 1f,
                    height = _scaledHeight
                };

                camera.rect = _rect;
            }
            else
            {
                float scaleWidth = 1f / _scaledHeight;

                Rect _rect = new Rect()
                {
                    x = (1f - scaleWidth) / 2f,
                    y = 0,
                    width = scaleWidth,
                    height = 1f
                };

                camera.rect = _rect;
            }
        }
        #endregion
    }
}
