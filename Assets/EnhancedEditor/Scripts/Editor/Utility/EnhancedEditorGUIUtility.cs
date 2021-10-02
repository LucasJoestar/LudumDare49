// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;
using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Contains multiple editor-related GUI utility methods, variables and properties.
    /// </summary>
    public static class EnhancedEditorGUIUtility
    {
        #region Global Editor Variables
        /// <summary>
        /// Default size (for both width and height) used to draw an asset preview (in pixels).
        /// </summary>
        public const float AssetPreviewDefaultSize = 64f;

        /// <summary>
        /// Width used to draw foldout buttons (in pixels).
        /// </summary>
        public const float FoldoutWidth = 15f;

        /// <summary>
        /// Width used to draw various icons (in pixels).
        /// </summary>
        public const float IconWidth = 20f;

        /// <summary>
        /// Default height used to draw a help box.
        /// </summary>
        public const float DefaultHelpBoxHeight = 38f;

        /// <summary>
        /// Default width of the lines surrounding the label of a section (in pixels).
        /// </summary>
        public const float SectionDefaultLineWidth = 50f;

        /// <summary>
        /// Space on both sides of a section between its label and the horizontal lines (in pixels).
        /// </summary>
        public const float SectionLabelMargins = 5f;

        /// <summary>
        /// Size (both width and height) of the icons drawn using the styles
        /// <see cref="EnhancedEditorStyles.OlMinus"/> and <see cref="EnhancedEditorStyles.OlPlus"/>.
        /// </summary>
        public const float OlStyleSize = 16f;
        #endregion

        #region Color
        /// <summary>
        /// Color used to draw peer background lines.
        /// </summary>
        public static readonly EditorColor GUIPeerLineColor = new EditorColor(new Color(.72f, .72f, .72f),
                                                                              new Color(.195f, .195f, .195f));

        /// <summary>
        /// Color used for various selected GUI controls.
        /// </summary>
        public static readonly EditorColor GUISelectedColor = new EditorColor(new Color(0f, .5f, 1f, .28f),
                                                                              new Color(0f, .5f, 1f, .25f));

        /// <summary>
        /// Color used for link labels, when the mouse is not hover.
        /// </summary>
        public static readonly EditorColor LinkLabelNormalColor = new EditorColor(new Color(0f, .235f, .533f, 1f),
                                                                                  new Color(.506f, .706f, 1f, 1f));
        /// <summary>
        /// Color used for link labels, when the mouse is hover.
        /// </summary>
        public static readonly EditorColor LinkLabelActiveColor = new EditorColor(new Color(.12f, .53f, 1f, 1f),
                                                                                  new Color(.9f, .9f, .9f, 1f));

        /// <summary>
        /// Editor GUI background color used in dark theme.
        /// </summary>
        public static readonly Color DarkThemeGUIBackgroundColor = new Color32(56, 56, 56, 255);

        /// <summary>
        /// Editor GUI background color used in light theme.
        /// </summary>
        public static readonly Color LightThemeGUIBackgroundColor = new Color32(194, 194, 194, 255);

        /// <summary>
        /// Current editor GUI background color, depending on whether currently using the light theme or the dark theme.
        /// </summary>
        public static Color GUIThemeBackgroundColor
        {
            get
            {
                Color _color = EditorGUIUtility.isProSkin
                                ? DarkThemeGUIBackgroundColor
                                : LightThemeGUIBackgroundColor;

                return _color;
            }
        }
        #endregion

        #region GUI Content
        private static readonly GUIContent labelGUI = new GUIContent(GUIContent.none);

        // -----------------------

        /// <summary>
        /// Get the <see cref="GUIContent"/> label associated with a specific <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="_property"><see cref="SerializedProperty"/> to get label from.</param>
        /// <returns>Label associated with the property.</returns>
        public static GUIContent GetPropertyLabel(SerializedProperty _property)
        {
            string _name = ObjectNames.NicifyVariableName(_property.name);

            labelGUI.text = _name;
            labelGUI.tooltip = _property.tooltip;

            return labelGUI;
        }

        /// <summary>
        /// Get a cached <see cref="GUIContent"/> for a specific label.
        /// </summary>
        /// <param name="_label"><see cref="GUIContent"/> text label.</param>
        /// <returns><see cref="GUIContent"/> to use.</returns>
        public static GUIContent GetLabelGUI(string _label)
        {
            labelGUI.text = _label;
            labelGUI.tooltip = string.Empty;

            return labelGUI;
        }
        #endregion

        #region Event and Clicks
        /// <summary>
        /// Did the user just performed a context click on this position?
        /// </summary>
        /// <param name="_position"><inheritdoc cref="DocumentationMethod(Rect)" path="/param[@name='_position']"/></param>
        /// <returns>True if the user performed a context click here, false otherwise.</returns>
        public static bool ContextClick(Rect _position)
        {
            if (_position.Event(out Event _event) == EventType.ContextClick)
            {
                _event.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Did the user just pressed a mouse button on this position?
        /// </summary>
        /// <param name="_position"><inheritdoc cref="DocumentationMethod(Rect)" path="/param[@name='_position']"/></param>
        /// <returns>True if the user clicked here, false otherwise.</returns>
        public static bool MouseDown(Rect _position)
        {
            if (_position.Event(out Event _event) == EventType.MouseDown)
            {
                _event.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Did the user just released the main mouse button on this position?
        /// </summary>
        /// <param name="_position"><inheritdoc cref="DocumentationMethod(Rect)" path="/param[@name='_position']"/></param>
        /// <returns>True if the user released this mouse button here, false otherwise.</returns>
        public static bool MainMouseUp(Rect _position)
        {
            if ((_position.Event(out Event _event) == EventType.MouseUp) && (_event.button == 0))
            {
                _event.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Did the user just performed a click to deselect element(s) on this position?
        /// </summary>
        /// <param name="_position"><inheritdoc cref="DocumentationMethod(Rect)" path="/param[@name='_position']"/></param>
        /// <returns>True if the user performed a deselection click here, false otherwise.</returns>
        public static bool DeselectionClick(Rect _position)
        {
            if ((_position.Event(out Event _event) == EventType.MouseDown) && !_event.control && !_event.shift && (_event.button == 0))
            {
                _event.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Did the user just performed a click to select element(s) on this position?
        /// <br/>
        /// If so, all necesary elements will be automatically selected.
        /// </summary>
        /// <param name="_position"><inheritdoc cref="DocumentationMethod(Rect)" path="/param[@name='_position']"/></param>
        /// <param name="_array">All elements that can be selected.</param>
        /// <param name="_index">Index of the current array element.</param>
        /// <param name="_isElementSelected">Used to know if a specific element is selected.</param>
        /// <param name="_onSelect">Callback when selecting an element.</param>
        /// <returns>True if the user performed a selection click here, false otherwise.</returns>
        public static bool SelectionClick(Rect _position, Array _array, int _index, Predicate<int> _isElementSelected, Action<int, bool> _onSelect)
        {
            // Only select on mouse down.
            if (_position.Event(out Event _event) != EventType.MouseDown)
                return false;

            if (_event.shift)
            {
                int _firstIndex = -1;
                int _lastIndex = -1;

                // Find first index.
                for (int _i = 0; _i < _array.Length; _i++)
                {
                    if (_isElementSelected(_i) || (_i == _index))
                    {
                        _firstIndex = _i;
                        break;
                    }
                }

                // Find last index.
                for (int _i = _array.Length; _i-- > 0;)
                {
                    if (_isElementSelected(_i) || (_i == _index))
                    {
                        _lastIndex = _i + 1;
                        break;
                    }
                }

                // Select all elements between indexes.
                for (int _i = _firstIndex; _i < _lastIndex; _i++)
                {
                    _onSelect(_i, true);
                }
            }
            else if (_event.control)
            {
                // Inverse selected state.
                bool _isSelected = _isElementSelected(_index);
                _onSelect(_index, !_isSelected);
            }
            else if (!_isElementSelected(_index) || (_event.button == 0))
            {
                // Unselect every element except this one.
                for (int _i = 0; _i < _array.Length; _i++)
                {
                    bool _isSelected = _i == _index;
                    _onSelect(_i, _isSelected);
                }
            }

            _event.Use();
            return true;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Repaints all editors associated with a specific <see cref="SerializedObject"/>.
        /// </summary>
        /// <param name="_object"><see cref="SerializedObject"/> to repaint associated editor(s).</param>
        public static void Repaint(SerializedObject _object)
        {
            UnityEditor.Editor[] _editors = ActiveEditorTracker.sharedTracker.activeEditors;
            foreach (UnityEditor.Editor _editor in _editors)
            {
                if (_editor.serializedObject == _object)
                {
                    _editor.Repaint();
                }
            }
        }
        #endregion

        #region Documentation
        /// <summary>
        /// This method is for documentation only, used by inheriting its parameters documentation to centralize it in one place.
        /// </summary>
        /// <param name="_position">Rectangle on the screen where to check for user actions.</param>
        internal static void DocumentationMethod(Rect _position) { }
        #endregion
    }
}
