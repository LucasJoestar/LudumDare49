// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Special drawer for fields with the attribute <see cref="EndFoldoutAttribute"/> (inherit from <see cref="EnhancedPropertyDrawer"/>).
    /// </summary>
    [CustomDrawer(typeof(EndFoldoutAttribute))]
	public class EndFoldoutPropertyDrawer : EnhancedPropertyDrawer
    {
        #region Drawer Content
        private static List<EndFoldoutPropertyDrawer> endFoldouts = new List<EndFoldoutPropertyDrawer>();

        private BeginFoldoutPropertyDrawer begin = null;
        private Guid guid = default;
        private float height = 0f;

        private bool isFirstDraw = true;

        // -----------------------

        public override void OnEnable()
        {
            EndFoldoutAttribute _attribute = Attribute as EndFoldoutAttribute;
            guid = _attribute.guid;

            // Try to reconnect this foldout, as some properties can be recreated while
            // already existing (like the ObjectReference type properties).
            for (int _i = endFoldouts.Count; _i-- > 0;)
            {
                EndFoldoutPropertyDrawer _foldout = endFoldouts[_i];

                // Remove null entries.
                if (_foldout == null)
                {
                    endFoldouts.RemoveAt(_i);
                    continue;
                }

                // If an existing foldout is found with the same guid, replace it.
                if (_foldout.guid == guid)
                {
                    begin = _foldout.begin;
                    endFoldouts[_i] = this;

                    _foldout.begin = null;
                    return;
                }
            }

            // Only register this foldout if it has a beginning.
            if (BeginFoldoutPropertyDrawer.GetFoldout(out begin))
            {
                endFoldouts.Add(this);
            }
        }

        public override void OnAfterGUI(Rect _position, SerializedProperty _property, GUIContent _label, out float _height)
        {
            // Ignore this foldout if it has no beginning.
            if (begin == null)
            {
                _height = 0f;
                return;
            }

            // Get the full foldout group state and position.
            bool _foldout = begin.PopFoldout(_position.y, height, out float _beginPos, out float _fade, out bool _hasColor);

            // Again, only update the foldout position on Repaint event.
            if (Event.current.type == EventType.Repaint)
            {
                height = (_beginPos - _position.y) * (1f - _fade);

                // Calculates the new drawing position.
                _position = EditorGUI.IndentedRect(_position);

                _position.x -= EnhancedEditorGUIUtility.FoldoutWidth;
                _position.width += EnhancedEditorGUIUtility.FoldoutWidth + 2f;
                _position.height = height + (_foldout ? 0f : 2f);

                // Fill any blank in this color group during transitions.
                Color _color = BeginFoldoutPropertyDrawer.PopColor();
                if ((height != 0f) && (_fade > 0f))
                {
                    EditorGUI.DrawRect(_position, _color);
                }

                // Repaint over leaking colors.
                if (_color == EnhancedEditorGUIUtility.GUIThemeBackgroundColor)
                {
                    _position.y += Screen.height;
                    _position.height -= Screen.height - (_foldout ? 2f : 0f);

                    EditorGUI.DrawRect(_position, _color);
                }
            }
            else if (isFirstDraw)
            {
                height = (_beginPos - _position.y) * (1f - _fade);
                isFirstDraw = false;
            }

            // Add some spacing for color groups.
            _height = height;
            if (_hasColor)
            {
                _height += EditorGUIUtility.standardVerticalSpacing + 1f;
            }
        }
        #endregion

        #region Utility
        internal static bool ReconnectFoldout(BeginFoldoutPropertyDrawer _beginFoldout, string _id)
        {
            for (int _i = endFoldouts.Count; _i-- > 0;)
            {
                EndFoldoutPropertyDrawer _endFoldout = endFoldouts[_i];
                if (_endFoldout == null)
                {
                    endFoldouts.RemoveAt(_i);
                    continue;
                }

                if (_endFoldout.begin.id == _id)
                {
                    _endFoldout.begin = _beginFoldout;
                    return true;
                }
            }

            return false;
        }

        internal static bool IsConnected(BeginFoldoutPropertyDrawer _beginFoldout)
        {
            for (int _i = endFoldouts.Count; _i-- > 0;)
            {
                EndFoldoutPropertyDrawer _endFoldout = endFoldouts[_i];
                if (_endFoldout == null)
                {
                    endFoldouts.RemoveAt(_i);
                    continue;
                }

                if (_endFoldout.begin == _beginFoldout)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
