using System.IO;
using Starflare.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Starflare.EditorTools
{
    /// <summary>
    /// Rebuilds the prototype scene and player settings from code, so neither has to be
    /// hand-edited. Run from the Starflare menu, or headless:
    /// Unity -batchmode -quit -projectPath game -executeMethod Starflare.EditorTools.ProjectSetup.Run
    /// </summary>
    public static class ProjectSetup
    {
        const string ScenePath = "Assets/Scenes/Blackjack.unity";

        [MenuItem("Starflare/Rebuild Project Setup")]
        public static void Run()
        {
            ConfigurePlayer();
            BuildBlackjackScene();
        }

        static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "Starflare";
            PlayerSettings.productName = "Starflare Casino";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
        }

        static void BuildBlackjackScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = new GameObject("Main Camera", typeof(Camera));
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;

            new GameObject("Blackjack Table", typeof(BlackjackTable));

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
        }
    }
}
