// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// <see cref="GameObject"/> and <see cref="Component"/> picker window, only displaying objects with specific components and interfaces.
    /// </summary>
    public class ObjectPickerWindow : EditorWindow
    {
        #region Object Info
        private struct ObjectInfo
        {
            public GameObject Object;
            public string Name;
            public bool IsVisible;

            // -----------------------

            public ObjectInfo(GameObject _object, string _name)
            {
                Object = _object;
                Name = _name;
                IsVisible = true;
            }

            public ObjectInfo(GameObject _object) : this(_object, _object.name) { }
        }
        #endregion

        #region Window GUI
        /// <summary>
        /// Creates and shows a new <see cref="Component"/> picker window, only displaying objects with specific components and interfaces.
        /// </summary>
        /// <param name="_id">Picker associated control id. Use it to get newly selected object with <see cref="GetSelectedObject(int, out Component)"/>).</param>
        /// <param name="_objectType">Selected object type.</param>
        /// <inheritdoc cref="GetWindow(int, GameObject, Type[], bool, Action{GameObject}"/>
        public static ObjectPickerWindow GetWindow(int _id, Component _selectedObject, Type _objectType, Type[] _requiredTypes, bool _allowSceneObjects,
                                                   Action<Component> _onSelectObject = null)
        {
            if (!ArrayUtility.Contains(_requiredTypes, _objectType))
            {
                ArrayUtility.Add(ref _requiredTypes, _objectType);
            }

            GameObject _gameObject = (_selectedObject != null)
                                    ? _selectedObject.gameObject
                                    : null;


            ObjectPickerWindow _window = GetWindow(_id, _gameObject, _requiredTypes, _allowSceneObjects, (go) =>
            {
                Component _component = GetSelectedComponent(go);
                _onSelectObject?.Invoke(_component);
            });

            objectType = _objectType;
            return _window;
        }

        /// <summary>
        /// Creates and shows a new <see cref="GameObject"/> picker window, only displaying objects with specific components or interfaces.
        /// </summary>
        /// <param name="_id">Picker associated control id. Use it to get newly selected object with <see cref="GetSelectedObject(int, out GameObject)"/>).</param>
        /// <param name="_selectedObject">The currently selected object in picker.</param>
        /// <param name="_requiredTypes">Only the objects possessing all of these required components will be displayed
        /// (must either be a component or an interface).</param>
        /// <param name="_allowSceneObjects">Allow or not to assign scene objects.</param>
        /// <param name="_onSelectObject">Callback whenever a new object is selected by the user.</param>
        /// <returns><see cref="ObjectPickerWindow"/> instance on screen.</returns>
        public static ObjectPickerWindow GetWindow(int _id, GameObject _selectedObject, Type[] _requiredTypes, bool _allowSceneObjects, Action<GameObject> _onSelectObject = null)
        {
            ObjectPickerWindow _window = GetWindow<ObjectPickerWindow>(true, "GameObject Picker", true);
            _window.Initialize(_id, _selectedObject, _requiredTypes, _allowSceneObjects, _onSelectObject);
            _window.Show();

            objectType = null;
            return _window;
        }

        // -------------------------------------------
        // Window GUI
        // -------------------------------------------

        private const float LineHeight = 16f;
        private const string SearchFieldControlName = "SearchFilter";

        private static int id = 0;
        private static bool hasSelectedObject = false;
        private static GameObject selectedObject = null;
        private static Type objectType = null;

        private readonly ObjectInfo nullObject = new ObjectInfo(null, "None");
        private readonly GUIContent[] tabsGUI = new GUIContent[]
                                                    {
                                                        new GUIContent("Assets", "All matching asset objects."),
                                                        new GUIContent("Scene", "All matching scene objects.")
                                                    };

        private GUIStyle PickerTabStyle = null;
        private GUIStyle PickerBackgroundStyle = null;

        private ObjectInfo[] assetObjects = new ObjectInfo[] { };
        private ObjectInfo[] sceneObjects = new ObjectInfo[] { };
        private bool allowSceneObjects = true;

        private Type[] requiredTypes = null;
        private Action<GameObject> onSelectObject = null;

        private string searchFilter = string.Empty;
        private bool doFocusSearchField = true;

        private Vector2 scroll = new Vector2();
        private int selectedTab = 0;

        // -----------------------

        private void OnGUI()
        {
            // Search field.
            using (var _scope = new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUI.SetNextControlName(SearchFieldControlName);

                string _searchFilter = EnhancedEditorGUILayout.ToolbarSearchField(searchFilter);
                if (_searchFilter != searchFilter)
                {
                    searchFilter = _searchFilter;

                    FilterObjects(assetObjects);
                    FilterObjects(sceneObjects);
                }

                // Only focus the field once after it has been drawn.
                if (doFocusSearchField)
                {
                    GUI.FocusControl(SearchFieldControlName);
                    doFocusSearchField = false;
                }
            }

            // Tab selection.
            using (var _scope = new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Using an array iteration instead of two direct toggles allows to easily add new tabs whenever we want.
                int _selectedTab = selectedTab;
                for (int _i = 0; _i < tabsGUI.Length; _i++)
                {
                    // Simply ignore scene tab in this case.
                    if (!allowSceneObjects && (_i == 1))
                        continue;

                    GUIContent _tabGUI = tabsGUI[_i];
                    bool _isSelected = _selectedTab == _i;

                    if (GUILayout.Toggle(_isSelected, _tabGUI, PickerTabStyle))
                    {
                        _selectedTab = _i;
                    }
                }

                if (_selectedTab != selectedTab)
                {
                    selectedTab = _selectedTab;
                    scroll = Vector2.zero;
                }

                // Use this to draw the bottom line.
                GUILayout.Label(GUIContent.none, PickerTabStyle, GUILayout.ExpandWidth(true));
            }

            // Background color.
            Rect _position = new Rect(0f, EditorGUILayout.GetControlRect(false, 0f).y - EditorGUIUtility.standardVerticalSpacing, position.width, position.height);
            GUI.Label(_position, GUIContent.none, PickerBackgroundStyle);

            // Object picker.
            using (var _scope = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = _scope.scrollPosition;
                switch (selectedTab)
                {
                    case 0:
                        DrawPicker(assetObjects);
                        break;

                    case 1:
                        DrawPicker(sceneObjects);
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
        #endregion

        #region Initialization
        private void Initialize(int _id, GameObject _selectedObject, Type[] _requiredTypes, bool _allowSceneObjects, Action<GameObject> _onSelectObject)
        {
            // Only keep eligible types.
            for (int _i = _requiredTypes.Length; _i-- > 0;)
            {
                Type _type = _requiredTypes[_i];
                if (!EnhancedEditorUtility.IsComponentOrInterface(_type))
                {
                    ArrayUtility.RemoveAt(ref _requiredTypes, _i);
                }
            }

            // Settings.
            id = _id;
            selectedObject = _selectedObject;
            requiredTypes = _requiredTypes;
            onSelectObject = _onSelectObject;
            allowSceneObjects = _allowSceneObjects;

            // Styles.
            PickerTabStyle = new GUIStyle("ObjectPickerTab");
            PickerBackgroundStyle = new GUIStyle("ProjectBrowserIconAreaBg");

            // Objects.
            assetObjects = GetMatchingObjects(EnhancedEditorUtility.LoadAssets<GameObject>());
            if (_allowSceneObjects)
            {
                sceneObjects = GetMatchingObjects(FindObjectsOfType<GameObject>());
                selectedTab = 1;
            }
            else
            {
                sceneObjects = new ObjectInfo[] { };
                selectedTab = 0;
            }
        }

        private ObjectInfo[] GetMatchingObjects(GameObject[] _objects)
        {
            List<ObjectInfo> _matchingObjects = new List<ObjectInfo>();
            foreach (GameObject _gameObject in _objects)
            {
                if (IsObjectValid(_gameObject))
                {
                    ObjectInfo _object = new ObjectInfo(_gameObject);
                    _matchingObjects.Add(_object);
                }
            }

            _matchingObjects.Sort((a, b) => a.Name.CompareTo(b.Name));
            _matchingObjects.Insert(0, nullObject);

            return _matchingObjects.ToArray();

            // ----- Local Method ----- \\

            bool IsObjectValid(GameObject _object)
            {
                foreach (Type _type in requiredTypes)
                {
                    if (!_object.GetComponent(_type))
                        return false;
                }

                return true;
            }
        }
        #endregion

        #region Object Picker
        private void DrawPicker(ObjectInfo[] _objects)
        {
            // Draw all visible objects.
            using (var _scope = new EditorGUI.IndentLevelScope())
            {
                for (int _i = 0; _i < _objects.Length; _i++)
                {
                    ObjectInfo _object = _objects[_i];
                    if (_object.IsVisible)
                    {
                        DrawObject(_object, _i);
                    }
                }
            }
        }

        private void DrawObject(ObjectInfo _object, int _index)
        {
            // Background color.
            bool _isSelected = selectedObject == _object.Object;
            Rect _position = new Rect(EditorGUILayout.GetControlRect(true, LineHeight))
            {
                x = 0f,
                width = position.width
            };

            EnhancedEditorGUI.BackgroundLine(_position, _isSelected, _index);

            // Draw null object (first index) without icon.
            if (_index == 0)
            {
                _position.xMin += 15f;
                EditorGUI.LabelField(_position, _object.Name);
            }
            else
            {
                // Don't store the object content as it can need multiple calls for Unity to properly load it, and is already cached internally.
                EditorGUI.LabelField(_position, EditorGUIUtility.ObjectContent(_object.Object, typeof(GameObject)));
            }

            // Select on click.
            if (EnhancedEditorGUIUtility.MouseDown(_position))
            {
                SelectObject(_object.Object);
                if (Event.current.clickCount > 1)
                {
                    Close();
                }
            }
        }
        #endregion

        #region Get Selected Object
        /// <inheritdoc cref="GetSelectedObject(int, out GameObject)"/>
        public static bool GetSelectedObject(int _id, out Component _object)
        {
            if (!GetSelectedObject(_id, out GameObject _gameObject) || (objectType == null))
            {
                _object = null;
                return false;
            }

            _object = GetSelectedComponent(_gameObject);
            return true;
        }

        /// <summary>
        /// Get the newly selected object by the user for a specific control id.
        /// </summary>
        /// <param name="_id">Control id to get selected object for.</param>
        /// <param name="_object">Newly selected object.</param>
        /// <returns>True if the user selected a new object, false otherwise.</returns>
        public static bool GetSelectedObject(int _id, out GameObject _object)
        {
            if (hasSelectedObject && (_id == id))
            {
                _object = selectedObject;
                hasSelectedObject = false;

                GUI.changed = true;
                return true;
            }

            _object = null;
            return false;
        }
        #endregion

        #region Utility
        private static Component GetSelectedComponent(GameObject _object)
        {
            Component _component = _object?.GetComponent(objectType);
            return _component;
        }

        private void FilterObjects(ObjectInfo[] _objects)
        {
            string _searchFilter = searchFilter.ToLower();
            for (int _i = 1; _i < _objects.Length; _i++)
            {
                _objects[_i].IsVisible = _objects[_i].Name.ToLower().Contains(_searchFilter);
            }
        }

        private void SelectObject(GameObject _object)
        {
            selectedObject = _object;
            hasSelectedObject = true;
            onSelectObject?.Invoke(_object);

            InternalEditorUtility.RepaintAllViews();
        }
        #endregion
    }
}
