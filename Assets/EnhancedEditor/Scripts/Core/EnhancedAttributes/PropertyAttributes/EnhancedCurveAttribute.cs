// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using UnityEngine;

namespace EnhancedEditor
{
    /// <summary>
    /// Displays this <see cref="AnimationCurve"/> with some extra configurable options.
    /// </summary>
	public class EnhancedCurveAttribute : EnhancedPropertyAttribute
    {
        #region Global Members
        public const SuperColor DefaultColor = SuperColor.Sapphire;

        /// <summary>
        /// Curve color.
        /// </summary>
        public readonly Color Color = default;

        /// <summary>
        /// Bounds used to draw this curve representation.
        /// </summary>
        public readonly Rect Rect = new Rect(0f, 0f, 0f, 0f);

        // -----------------------

        /// <param name="_color">Curve color.</param>
        /// <inheritdoc cref="EnhancedCurveAttribute(float, float, float, float, SuperColor)"/>
        public EnhancedCurveAttribute(SuperColor _color = DefaultColor)
        {
            Color = _color.Get();
        }

        /// <param name="_minX">Minimum curve X-axis oriented value.</param>
        /// <param name="_minY">Minimum curve Y-axis oriented value.</param>
        /// <param name="_maxX">Maximum curve X-axis oriented value.</param>
        /// <param name="_maxY">Maximum curve Y-axis oriented value.</param>
        /// <param name="_color"><inheritdoc cref="Color" path="/summary"/></param>
        /// <inheritdoc cref="EnhancedCurveAttribute"/>
        public EnhancedCurveAttribute(float _minX, float _minY, float _maxX, float _maxY, SuperColor _color = DefaultColor) : this(_color)
        {
            Rect = new Rect(_minX, _minY, _maxX - _minX, _maxY - _minY);
        }
        #endregion
    }
}
