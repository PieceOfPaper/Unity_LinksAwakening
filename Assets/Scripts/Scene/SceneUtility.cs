using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtility
{

#if UNITY_EDITOR

    public static string GetEditorBuildSettingsScenePathBySceneName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return string.Empty;

        var lowerSceneName = sceneName.ToLower() + ".unity";
        var buildSettingScenes = UnityEditor.EditorBuildSettings.scenes;
        for (int i = 0; i < buildSettingScenes.Length; i ++)
        {
            var pathSplit = buildSettingScenes[i].path.Replace('\\','/').Split('/');
            if (pathSplit[pathSplit.Length - 1].StartsWith(sceneName) && pathSplit[pathSplit.Length - 1].ToLower() == lowerSceneName)
                return buildSettingScenes[i].path;
        }

        return string.Empty;
    }

    public static Scene GetEditorOpenedSceneBySceneName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return default;

        for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i ++)
        {
            var sceneData = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
            if (sceneData.IsValid() == false) continue;

            if (sceneData.name == sceneName)
                return sceneData;
        }
        return default;
    }

#endif
}
