// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using UnityEditor;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer (inheriting from <see cref="EnhancedPropertyDrawer"/>) for classes with attribute <see cref="ValidationMemberAttribute"/>.
    /// </summary>
    [CustomDrawer(typeof(ValidationMemberAttribute))]
    public class PropertyFieldPropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        public override void OnValueChanged()
        {
            ValidationMemberAttribute _attribute = (ValidationMemberAttribute)Attribute;
            if (!_attribute.Mode.IsActive())
                return;

            // Get new serialized property value, then use it to set the C# native property value.
            /*if (EnhancedEditorUtility.GetSerializedObjectMemberValue(_property.serializedObject, _property.name, out object _value))
            {
                string _propertyName = string.IsNullOrEmpty(_attribute.PropertyName)
                                        ? (char.ToUpper(_property.name[0]) + _property.name.Substring(1))
                                        : _attribute.PropertyName;

                EnhancedEditorUtility.SetPropertyValue(_property.serializedObject, _propertyName, _value);
            }*/
        }
        #endregion
    }
}
