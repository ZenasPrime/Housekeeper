#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ZenTools.Housekeeper
{
    /// <summary>
    /// Subscribes to the play mode state change event of the Unity Editor and
    /// invoke the OnPlaymodeStateChange method.
    /// </summary>
    [InitializeOnLoad]
    public static class PlaymodeStateHook
    {
        static PlaymodeStateHook()
        {
            EditorApplication.playModeStateChanged += HousekeeperMenu.OnPlaymodeStateChange;
        }
    }

    /// <summary>
    /// Subscribes to the active scene changed event of the Unity Editor
    /// and invokes the OnSceneChange method.
    /// </summary>
    [InitializeOnLoad]
    public static class SceneLoadHook
    {
        static SceneLoadHook()
        {
            EditorSceneManager.activeSceneChanged += HousekeeperMenu.OnSceneChange;
        }
    }

    public class HousekeeperMenu 
    {
        private static string lastActiveScenePath = "";
        private static bool isHousekeeperRun = false;

        /// <summary>
        /// Adds a new ZenTools/Housekeeper menu item called 'Run'.
        /// Gives the user the option to save the currently modified scenes.
        /// Enters Playmode using the scene listed at index 0 in the Build Settings.
        /// </summary>
        [MenuItem("ZenTools/Housekeeper/Run #&r")]
        private static void EnterPlaymodeWithScene0()
        {
            if (EditorApplication.isPlaying)
            {
                isHousekeeperRun = false;
                EditorApplication.ExitPlaymode();

                string scenePath = lastActiveScenePath;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }
            else
            {
                try
                {
                    // opens a dialog that asks the user if they want to save the current scene changes
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                    // get the current scene path and store it
                    lastActiveScenePath = EditorSceneManager.GetActiveScene().path;

                    // get the first scene in Build settings and set the Editor to use it when starting PlayMode
                    string scenePath = EditorBuildSettings.scenes[0].path;
                    EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                    // if we are entering PlayMode via the Housekeeper Run menu item
                    isHousekeeperRun = true;

                    // enter PlayMode
                    EditorApplication.EnterPlaymode();
                    
                }
                catch
                {
                    Debug.LogError("Please add a scene to the Build Settings");
                }
            }
        }

        /// <summary>
        /// Called when the Editor PlayMode state changes.
        /// isHousekeeperRun keeps tract if the current state
        /// change was initiated by the Housekeeper menu item
        /// </summary>
        /// <param name="stateChange">the current PlayMode state</param>
        public static void OnPlaymodeStateChange(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredPlayMode && !isHousekeeperRun)
            {
                string scenePath = lastActiveScenePath;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }

            if (stateChange == PlayModeStateChange.ExitingPlayMode && isHousekeeperRun)
            {
                string activeScene = SceneManager.GetActiveScene().path;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene);
            }
        }

        /// <summary>
        /// Called when the Editor changes the active scene.
        /// Sets the currently active scene to play when the
        /// user uses the Play button to enter PlayMode.
        /// </summary>
        /// <param name="current">not used</param>
        /// <param name="next">not used</param>
        public static void OnSceneChange(Scene current, Scene next)
        {
            string scenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        }

        /// <summary>
        /// Adds a new ZenTools/Housekeeper menu item called 'Clear PlayerPrefs'.
        /// Clears the currently save PlayerPrefs.
        /// </summary>
        [MenuItem("ZenTools/Housekeeper/Clear PlayerPrefs #&p")]
        private static void ClearPlayerPrefs()
        {
            Debug.Log("PlayerPrefs Cleared");
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif