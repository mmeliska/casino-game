using System.IO;
using Starflare.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

        const string SettingsDir = "Assets/Settings";
        const string RendererPath = SettingsDir + "/URP Renderer.asset";
        const string PipelinePath = SettingsDir + "/URP Pipeline.asset";

        [MenuItem("Starflare/Rebuild Project Setup")]
        public static void Run()
        {
            ConfigurePlayer();
            ConfigureRenderPipeline();
            BuildBlackjackScene();
        }

        /// <summary>URP replaces the deprecated Built-in pipeline for every quality level.</summary>
        static void ConfigureRenderPipeline()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                Directory.CreateDirectory(SettingsDir);
                var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }
            QualitySettings.SetQualityLevel(current, false);
            AssetDatabase.SaveAssets();
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
