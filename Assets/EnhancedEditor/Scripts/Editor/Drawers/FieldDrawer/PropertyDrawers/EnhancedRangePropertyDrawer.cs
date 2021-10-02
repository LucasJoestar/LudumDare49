// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer (inheriting from <see cref="EnhancedPropertyDrawer"/>) for classes with attribute <see cref="PrecisionSliderAttribute"/>.
    /// </summary>
    [CustomDrawer(typeof(PrecisionSliderAttribute))]
	public class EnhancedRangePropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        public override bool OnGUI(Rect _position, SerializedProperty _property, GUIContent _label, out float _height)
        {
            PrecisionSliderAttribute _attribute = (PrecisionSliderAttribute)Attribute;
            //EnhancedEditorGUI.EnhancedSliderField(_position, _property, _label, _attribute.MinValue, _attribute.MaxValue, _attribute.Precision, out _height);
            _height = 0f;
            return false;
        }
        #endregion
    }
}
