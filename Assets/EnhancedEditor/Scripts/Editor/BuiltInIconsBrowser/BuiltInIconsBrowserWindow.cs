// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// Editor window used to browse Unity built-in icons.
    /// </summary>
	public class BuiltInIconsBrowserWindow : EditorWindow
    {
        #region Icon Wrapper
        [Serializable]
        private class Icon
        {
            public string Name = string.Empty;
            public GUIContent IconContent = null;
            public GUIContent FullContent = null;

            public Vector2 Size = Vector2.zero;
            public float LargerSide = 0f;

            // -----------------------

            public Icon(string _name, GUIContent _content)
            {
                Texture _texture = _content.image;

                Name = _name;
                IconContent = _content;
                FullContent = new GUIContent(_content)
                {
                    text = $" {_name}"
                };

                Size.Set(_texture.width, _texture.height);
                LargerSide = Mathf.Max(Size.x, Size.y);
            }
        }
        #endregion

        #region Window GUI
        /// <summary>
        /// Returns the first <see cref="BuiltInIconsBrowserWindow"/> currently on screen.
        /// <para/>
        /// Creates and shows a new instance if there is none.
        /// </summary>
        /// <returns><see cref="BuiltInIconsBrowserWindow"/> instance on screen.</returns>
        [MenuItem("Enhanced Editor/Built-in Icons Browser", false, 10)]
        public static BuiltInIconsBrowserWindow GetWindow()
        {
            BuiltInIconsBrowserWindow _window = GetWindow<BuiltInIconsBrowserWindow>("Built-in Icons Browser");
            _window.Show();

            return _window;
        }

        // -------------------------------------------
        // Window GUI
        // -------------------------------------------

        private const float SortOptionsWidth = 120f;
        private const float DisplayedIconsWidth = 140f;
        private const float SizeSliderWidth = 50f;
        private const float SizeSliderMinValue = 1f;
        private const float SizeSliderMaxValue = 3f;

        private const float ListIconSize = 32f;
        private const float GridIconSize = 32f;
        private const float SmallIconSize = 24f;
        private const float LargeIconSize = 60f;

        private const float InspectorHeight = 125f;
        private const float InspectorPreviewSize = 100f;
        private const float CodeSnippetMargin = 100f;
        private const float CopyButtonWidth = 75f;

        private const string UndoRecordTitle = "Icons Browser Change";
        private const string DisplayedIconsFormat = "Displayed Icons: {0}";
        private const string SizeFormat = "Size: {0}x{1}";
        private const string CodeSnippetInfo = "You can use one of the following code snippets to load this icon:";

        private readonly string[] codeSnippetFormats = new string[]
                                                            {
                                                                "EditorGUIUtility.IconContent(\"{0}\")",
                                                                "EditorGUIUtility.FindTexture(\"{0}\")"
                                                            };

        private readonly GUIContent displayedIconsGUI = new GUIContent(string.Empty, "Total amount of currently displayed icons.");
        private readonly GUIContent[] sortOptionsGUI = new GUIContent[]
                                                            {
                                                                new GUIContent("Sort by name", "Sort the icons by their name."),
                                                                new GUIContent("Sort by size", "Sort the icons by their size."),
                                                            };

        private readonly GUIContent iconNameHeaderGUI = new GUIContent("Name:", "Name of the selected icon.");
        private readonly GUIContent copyCodeGUI = new GUIContent("Copy", "Copy this code snippet.");
        private readonly GUIContent iconNameGUI = new GUIContent(string.Empty, "Name of this icon.");
        private readonly GUIContent iconContentGUI = new GUIContent();
        private readonly GUIContent sizeGUI = new GUIContent(string.Empty, "Size of this icon.");
        private readonly GUIContent[] tabsGUI = new GUIContent[]
                                                            {
                                                                new GUIContent("All Icons"),
                                                                new GUIContent("Small"),
                                                                new GUIContent("Medium"),
                                                                new GUIContent("Large")
                                                            };

        private readonly EditorColor inspectorDarkColor = new EditorColor(new Color(.7f, .7f, .7f, 1f), new Color(.8f, .8f, .8f, 1f));
        private readonly EditorColor gridColor = new EditorColor(new Color(.9f, .9f, .9f, 1f), Color.white);
        private readonly EditorColor gridSelectedColor = new EditorColor(new Color(.7f, .7f, .7f, 1f), new Color(.75f, .75f, .75f, 1f));

        [SerializeField] private int selectedTab = 0;
        [SerializeField] private bool isInspectorDarkColor = false;

        [SerializeField] private Icon[] filteredIcons = new Icon[] { };
        [SerializeField] private string selectedIcon = string.Empty;

        [SerializeField] private int selectedSortOption = 0;
        [SerializeField] private bool doSortAscending = true;
        [SerializeField] private string searchFilter = string.Empty;
        [SerializeField] private float sizeSlider = 1f;

        private Icon[] icons = new Icon[] { };
        private Vector2 scroll = new Vector2();

        // -----------------------

        private void OnEnable()
        {
            List<Icon> _icons = new List<Icon>();
            Texture2D[] _textures;

            try
            {
                MethodInfo _method = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                object _bundle = _method.Invoke(null, null);

                AssetBundle _assetBundle = _bundle as AssetBundle;
                _textures = _assetBundle.LoadAllAssets<Texture2D>();
            }
            catch (Exception _e)
            {
                Debug.LogError("Error => " + _e);

                _textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            }

            // The most convenient way to detect built-in icons is to simply try to load them, as nothing in their path can tell them apart.
            // As Unity will automatically log an error when a load fails, let's disable the logger temporarily.
            Debug.unityLogger.logEnabled = false;

            foreach (Texture2D _texture in _textures)
            {
                string _iconName = _texture.name;
                GUIContent _content = EditorGUIUtility.IconContent(_iconName, _iconName);

                if ((_content != null) && (_content.image != null))
                {
                    // Remove the dark theme prefix of the icon, as it will load the same anyway.
                    if (_iconName.StartsWith("d_"))
                        _iconName = _iconName.Remove(0, 2);

                    // Avoid to load any duplicate.
                    if (_icons.Exists((t) => t.Name == _iconName))
                        continue;

                    Icon _icon = new Icon(_iconName, _content);
                    _icons.Add(_icon);
                }
            }

            Debug.unityLogger.logEnabled = true;

            icons = _icons.ToArray();

            Resources.UnloadUnusedAssets();
            GC.Collect();

            FilterIcons();
            UpdateSelectedIcon();

            // Undo callback.
            Undo.undoRedoPerformed -= UpdateSelectedIcon;
            Undo.undoRedoPerformed += UpdateSelectedIcon;
        }

        private void OnGUI()
        {
            Undo.RecordObject(this, UndoRecordTitle);

            DrawToolbar();
            DrawInspector();

            // Tabs to select the type of icons to be displayed.
            GUILayout.Space(5f);

            int _selectedTab = EnhancedEditorGUILayout.CenteredToolbar(selectedTab, tabsGUI, GUILayout.Height(25f));
            if (_selectedTab != selectedTab)
            {
                selectedTab = _selectedTab;
                FilterIcons();
            }

            GUILayout.Space(5f);

            // Finally, draw each filtered icons.
            DrawIcons();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UpdateSelectedIcon;
        }
        #endregion

        #region GUI Drawers
        private void DrawToolbar()
        {
            using (var _scope = new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Sorting options.
                using (var _changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    EnhancedEditorGUILayout.ToolbarSortOptions(ref selectedSortOption, ref doSortAscending, sortOptionsGUI, GUILayout.Width(SortOptionsWidth));
                    if (_changeCheck.changed)
                    {
                        SortIcons();
                    }
                }

                // Search field.
                string _searchFilter = EnhancedEditorGUILayout.ToolbarSearchField(searchFilter, GUILayout.MinWidth(100f));
                if (_searchFilter != searchFilter)
                {
                    searchFilter = _searchFilter;
                    FilterIcons();
                }

                // Total displayed icons.
                GUILayout.Label(displayedIconsGUI, EnhancedEditorStyles.LeftAlignedToolbarButton, GUILayout.Width(DisplayedIconsWidth));

                // Size slider.
                sizeSlider = GUILayout.HorizontalSlider(sizeSlider, SizeSliderMinValue, SizeSliderMaxValue, GUILayout.Width(SizeSliderWidth));

                GUILayout.Space(7f);
            }
        }

        private void DrawInspector()
        {
            using (var _scope = new GUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(InspectorHeight)))
            {
                using (var _verticalScope = new GUILayout.VerticalScope())
                {
                    // Icon name.
                    Rect _position = EditorGUILayout.GetControlRect();
                    _position.width = 50f;

                    EnhancedEditorGUI.UnderlinedLabel(_position, iconNameHeaderGUI, EditorStyles.boldLabel);

                    _position.x += _position.width;
                    _position.xMax = position.xMax;

                    GUI.Label(_position, iconNameGUI);

                    // Code snippets.
                    GUILayout.Space(10f);
                    EditorGUILayout.HelpBox(CodeSnippetInfo, UnityEditor.MessageType.Info);
                    GUILayout.Space(5f);

                    for (int _i = 0; _i < codeSnippetFormats.Length; _i++)
                    {
                        string _snippet = string.Format(codeSnippetFormats[_i], selectedIcon);

                        _position = EditorGUILayout.GetControlRect();
                        _position.xMin += Mathf.Min(position.width * .075f, CodeSnippetMargin);
                        _position.xMax -= CopyButtonWidth + 5f;

                        EditorGUI.TextField(_position, _snippet);
                        _position.xMin = _position.xMax + 5f;
                        _position.width = CopyButtonWidth;

                        if (GUI.Button(_position, copyCodeGUI, EditorStyles.miniButton))
                            EditorGUIUtility.systemCopyBuffer = _snippet;
                    }
                }

                GUILayout.Space(10f);

                // Icon preview.
                using (var _verticalScope = new EditorGUILayout.VerticalScope(GUILayout.Width(InspectorPreviewSize)))
                using (var _colorScope = EnhancedGUI.GUIBackgroundColor.Scope(isInspectorDarkColor ? inspectorDarkColor : gridColor))
                {
                    if (GUILayout.Button(iconContentGUI, EnhancedEditorStyles.Button, GUILayout.Width(InspectorPreviewSize), GUILayout.Height(InspectorPreviewSize)))
                    {
                        isInspectorDarkColor = !isInspectorDarkColor;
                    }

                    GUILayout.Label(sizeGUI, EnhancedEditorStyles.CenteredLabel);
                }
            }
        }

        private void DrawIcons()
        {
            using (var _scope = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = _scope.scrollPosition;

                // Display the icons as a list if the slider value is at its minimum, and as a grid otherwise.
                if (sizeSlider == SizeSliderMinValue)
                {
                    GUILayoutOption _height = GUILayout.Height(ListIconSize);
                    for (int _i = 0; _i < filteredIcons.Length; _i++)
                    {
                        Icon _icon = filteredIcons[_i];
                        Rect _position = new Rect(EditorGUILayout.GetControlRect(_height))
                        {
                            xMin = 0f,
                            xMax = position.xMax
                        };

                        // Background color.
                        bool _isSelected = _icon.Name == selectedIcon;
                        EnhancedEditorGUI.BackgroundLine(_position, _isSelected, _i);

                        _position.xMin += 5f;
                        _position.yMin += 2f;
                        _position.yMax -= 2f;

                        // Selection button.
                        if (GUI.Button(_position, _icon.FullContent, EditorStyles.label))
                        {
                            SelectIcon(_icon);
                        }
                    }
                }
                else
                {
                    // Remove vertical spacing from height as it will be automatically reserved by layout.
                    float _size = GridIconSize * sizeSlider;
                    GUILayoutOption _height = GUILayout.Height(_size - EditorGUIUtility.standardVerticalSpacing);

                    float _screenWidth = Screen.width - 15f;
                    float _gridCount = Mathf.Floor(_screenWidth / _size);
                    float _margin = (_screenWidth % _size) / 2f;
                    int _index = 0;

                    while (_index < filteredIcons.Length)
                    {
                        Rect _position = EditorGUILayout.GetControlRect(_height);
                        _position.xMin += _margin;
                        _position.width = _size;
                        _position.height += EditorGUIUtility.standardVerticalSpacing;

                        for (int _i = 0; _i < _gridCount; _i++)
                        {
                            Icon _icon = filteredIcons[_index];
                            Color _color = (_icon.Name == selectedIcon)
                                         ? gridSelectedColor
                                         : gridColor;

                            // Selection button.
                            using (var _colorScope = EnhancedGUI.GUIBackgroundColor.Scope(_color))
                            {
                                if (GUI.Button(_position, _icon.IconContent))
                                {
                                    SelectIcon(_icon);
                                }
                            }

                            // Increment index.
                            _index++;
                            if (_index == filteredIcons.Length)
                                break;

                            _position.x += _position.width;
                        }
                    }
                }
            }
        }
        #endregion

        #region Utility
        private void FilterIcons()
        {
            List<Icon> _filteredIcons = new List<Icon>();
            string _searchFilter = searchFilter.ToLower();
            bool _useSearchFilter = !string.IsNullOrEmpty(searchFilter);

            foreach (Icon _icon in icons)
            {
                switch (selectedTab)
                {
                    // All icons.
                    case 0:
                        break;

                    // Small.
                    case 1:
                        if (_icon.LargerSide > SmallIconSize)
                            continue;

                        break;

                    // Medium.
                    case 2:
                        if ((_icon.LargerSide < SmallIconSize) || (_icon.LargerSide > LargeIconSize))
                            continue;

                        break;

                    // Big.
                    case 3:
                        if (_icon.LargerSide < LargeIconSize)
                            continue;

                        break;

                    default:
                        break;
                }

                if (!_useSearchFilter || _icon.Name.ToLower().Contains(_searchFilter))
                {
                    _filteredIcons.Add(_icon);
                }
            }

            filteredIcons = _filteredIcons.ToArray();
            displayedIconsGUI.text = string.Format(DisplayedIconsFormat, filteredIcons.Length);

            SortIcons();
        }

        private void SortIcons()
        {
            switch (selectedSortOption)
            {
                // Name.
                case 0:
                    Array.Sort(filteredIcons, CompareByName);
                    break;

                // Size.
                case 1:
                    Array.Sort(filteredIcons, CompareBySize);
                    break;

                default:
                    break;
            }

            if (!doSortAscending)
                Array.Reverse(filteredIcons);

            // ----- Local Methods ----- \\

            int CompareByName(Icon _a, Icon _b)
            {
                return _a.Name.CompareTo(_b.Name);
            }

            int CompareBySize(Icon _a, Icon _b)
            {
                int _compare = _a.LargerSide.CompareTo(_b.LargerSide);
                if (_compare == 0)
                    _compare = _a.Name.CompareTo(_b.Name);

                return _compare;
            }
        }

        private void UpdateSelectedIcon()
        {
            Icon _selectedIcon = Array.Find(icons, (i) => i.Name == selectedIcon);
            if (_selectedIcon == null)
                _selectedIcon = filteredIcons[0];

            SelectIcon(_selectedIcon);
        }

        private void SelectIcon(Icon _icon)
        {
            // Use the name of the icon instead of a direct reference
            // to avoid strange behaviour between this reference and its position in the filtered icons array.
            selectedIcon = _icon.Name;

            iconNameGUI.text = _icon.Name;
            sizeGUI.text = string.Format(SizeFormat, _icon.Size.x, _icon.Size.y);
            iconContentGUI.image = _icon.IconContent.image;

            // Stop editing text field to correctly repaint all code snippets.
            EditorGUIUtility.editingTextField = false;
        }
        #endregion
    }
}
