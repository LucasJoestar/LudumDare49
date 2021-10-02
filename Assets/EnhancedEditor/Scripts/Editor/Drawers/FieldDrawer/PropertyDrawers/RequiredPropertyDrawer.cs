// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer (inheriting from <see cref="EnhancedPropertyDrawer"/>) for classes with attribute <see cref="RequiredAttribute"/>.
    /// </summary>
    [CustomDrawer(typeof(RequiredAttribute))]
    public class RequiredPropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        public override bool OnGUI(Rect _position, SerializedProperty _property, GUIContent _label, out float _height)
        {
            //EnhancedEditorGUI.RequiredHelpBox(_position, _property, out _height);
            _height = 0f;
            return false;
        }

        public override void OnContextMenu(GenericMenu _menu)
        {
            EnhancedEditorGUI.AddRequiredUtilityToMenu(0, SerializedProperty, _menu);
        }
        #endregion
    }
}
