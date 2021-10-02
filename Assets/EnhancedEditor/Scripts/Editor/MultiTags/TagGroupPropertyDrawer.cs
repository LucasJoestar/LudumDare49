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
    /// Custom <see cref="TagGroup"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(TagGroup), true)]
	public class TagGroupPropertyDrawer : PropertyDrawer
    {
        #region Drawer Content
        private float height = 0f;

        // -----------------------

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            // Only calculate height if it has not be set yet. Left offset is equal to 18 pixels, and right offset to 5.
            if (height == 0f)
            {
                Rect _position = new Rect(18f, 0f, EditorGUIUtility.currentViewWidth - 18f - 5f, EditorGUIUtility.singleLineHeight);
                float _height = _position.height + EnhancedEditorGUI.GetTagGroupExtraHeight(_position, _property, _label);

                return _height;
            }

            return height;
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            // As the full height is given on position, set it for one line only.
            _position.height = EditorGUIUtility.singleLineHeight;
            EnhancedEditorGUI.TagGroupField(_position, _property, _label, out float _extraHeight);

            // Only save height on repaint event as it doesn't have any accurate value during layout.
            if (Event.current.type == EventType.Repaint)
            {
                height = _position.height + _extraHeight;
            }
        }
        #endregion
    }
}
