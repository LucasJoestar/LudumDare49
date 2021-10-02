// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EnhancedEditor.Editor
{
    /// <summary>
    /// <see cref="EnhancedEditor"/>-related preferences settings, independent to each user.
    /// <para/>
    /// Note that this should not be called on a <see cref="ScriptableObject"/> constructor, due to Unity preferences limitations.
    /// </summary>
    [Serializable]
	public class EnhancedEditorPreferences
    {
        #region Preferences Content
        /// <summary>
        /// The directory where to create all project auto-managed resources.
        /// </summary>
        public string AutoManagedResourceDefaultDirectory = "EnhancedEditor/AutoManagedResources";

        /// <summary>
        /// Directory used in the <see cref="BuildPipelineWindow"/> to build the game and look for existing builds.
        /// </summary>
        public string BuildDirectory = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Builds").Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        #endregion

        #region Preference Instance
        private const string PreferencesPath = "Preferences/EnhancedEditor";
        private const string PreferencesKey = "EnhancedEditorPreferences";

        private static EnhancedEditorPreferences preferences = new EnhancedEditorPreferences();
        private static bool isLoaded = false;

        /// <inheritdoc cref="EnhancedEditorPreferences"/>
        public static EnhancedEditorPreferences Preferences
        {
            // As preferences are user-dependant and should not be shared within the project,
            // EditorPrefs are used to save and get these datas instead of a ScriptableObject instance.
            get
            {
                if (!isLoaded)
                {
                    // Loads saved datas from EditorPrefs.
                    string _data = EditorPrefs.GetString(PreferencesKey, string.Empty);
                    if (!string.IsNullOrEmpty(_data))
                        JsonUtility.FromJsonOverwrite(_data, preferences);

                    isLoaded = true;
                }

                return preferences;
            }
            internal set
            {
                preferences = value;

                string _data = JsonUtility.ToJson(value);
                EditorPrefs.SetString(PreferencesKey, _data);
            }
        }
        #endregion

        #region Menu Navigation
        private static readonly GUIContent settingsGUI = new GUIContent(EditorGUIUtility.FindTexture("d_Settings"), "Opens the EnhancedEditor preferences settings.");

        // -----------------------

        /// <summary>
        /// Opens the Preferences window at the <see cref="EnhancedEditor"/> settings.
        /// </summary>
        [MenuItem("Enhanced Editor/Preferences", false, -50)]
        public static void OpenPreferencesSettings()
        {
            SettingsService.OpenUserPreferences(PreferencesPath);
        }

        [EditorToolbarRightExtension(Order = 500)]
        #pragma warning disable IDE0051
        private static void OpenPreferences()
        {
            GUILayout.FlexibleSpace();

            if (EnhancedEditorToolbar.Button(settingsGUI, GUILayout.Width(32f)))
            {
                OpenPreferencesSettings();
            }

            GUILayout.Space(25f);
        }
        #endregion

        #region Preferences Settings
        public static readonly string Label = "Enhanced Editor";
        public static readonly string[] Keywords = new string[]
                                                        {
                                                            "Enhanced",
                                                            "Editor",
                                                            "Build Pipeline",
                                                            "Autosave"
                                                        };

        // -----------------------

        [SettingsProvider]
        #pragma warning disable IDE0051
        private static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider _provider = new SettingsProvider(PreferencesPath, SettingsScope.User)
            {
                label = Label,

                guiHandler = (string _searchContext) =>
                {
                    // Load and save preferences settings.
                    EnhancedEditorPreferences _preferences = Preferences;

                    GUILayout.Space(10f);
                    EditorGUI.BeginChangeCheck();

                    DrawPreferences(_preferences);

                    if (EditorGUI.EndChangeCheck())
                        Preferences = preferences;
                },

                keywords = Keywords,
            };

            return _provider;
        }
        #endregion

        #region GUI
        public const string AutoManagedResourceDirectoryPanelTitle = "Auto-Managed Resources Default Directory";
        public const string BuildDirectoryPanelTitle = "Build Directory";

        public static readonly GUIContent AutoManagedResourceDirectoryGUI = new GUIContent("Auto-Managed Resource Default Directory",
                                                                                           "Default directory where are created all project auto-managed resources.");
        public static readonly GUIContent BuildDirectoryGUI = new GUIContent("Build Directory", "Directory where to build the game and look for existing builds.");

        // -----------------------

        private static void DrawPreferences(EnhancedEditorPreferences _preferences)
        {
            // Auto-managed resource default directory.
            _preferences.AutoManagedResourceDefaultDirectory = EnhancedEditorGUILayout.FolderField(AutoManagedResourceDirectoryGUI,
                                                                                                   _preferences.AutoManagedResourceDefaultDirectory, false,
                                                                                                   AutoManagedResourceDirectoryPanelTitle);

            // Build dirctory.
            DrawBuildDirectory(BuildDirectoryGUI);
        }

        internal static bool DrawBuildDirectory(GUIContent _content)
        {
            EnhancedEditorPreferences _preferences = Preferences;
            string _directory = EnhancedEditorGUILayout.FolderField(_content, _preferences.BuildDirectory, true, BuildDirectoryPanelTitle);

            if (_directory != _preferences.BuildDirectory)
            {
                _preferences.BuildDirectory = _directory;
                Preferences = _preferences;

                return true;
            }

            return false;
        }
        #endregion
    }
}
