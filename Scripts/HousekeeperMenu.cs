using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ZenTools.Housekeeper
{
    public class HousekeeperMenu 
    {
        /// <summary>
        /// Adds a new ZenTools/Housekeeper menu item called 'Run'.
        /// Gives the user the option to save the currently modified scenes.
        /// Enters Playmode using the scene listed at index 0 in the Build Settings.
        /// </summary>
        [MenuItem("ZenTools/Housekeeper/Run #&r")]
        private static void EnterPlaymodeWithScene0()
        {
            try
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                string scenePath = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                EditorApplication.EnterPlaymode();
            }
            catch
            {
                Debug.LogError("Please add a scene to the Build Settings");
            }

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
