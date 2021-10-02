// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using UnityEditor;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer (inheriting from <see cref="EnhancedPropertyDrawer"/>) for classes with attribute <see cref="MaxAttribute"/>.
    /// </summary>
    [CustomDrawer(typeof(MaxAttribute))]
    public class MaxPropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        public override void OnValueChanged()
        {
            MaxAttribute _attribute = (MaxAttribute)Attribute;
            EnhancedEditorUtility.CeilSerializedPropertyValue(SerializedProperty, _attribute.MaxValue);
        }
        #endregion
    }
}
