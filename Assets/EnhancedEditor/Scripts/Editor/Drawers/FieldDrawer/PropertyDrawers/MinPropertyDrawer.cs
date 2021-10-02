// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using UnityEditor;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer (inheriting from <see cref="EnhancedPropertyDrawer"/>) for classes with attribute <see cref="MinAttribute"/>.
    /// </summary>
    [CustomDrawer(typeof(MinAttribute))]
    public class MinPropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        public override void OnValueChanged()
        {
            MinAttribute _attribute = (MinAttribute)Attribute;
            EnhancedEditorUtility.FloorSerializedPropertyValue(SerializedProperty, _attribute.MinValue);
        }
        #endregion
    }
}
