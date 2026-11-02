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
    /// Rebuilds the prototype scenes and player settings from code, so neither has to be
    /// hand-edited. Run from the Starflare menu, or headless:
    /// Unity -batchmode -quit -projectPath game -executeMethod Starflare.EditorTools.ProjectSetup.Run
    /// </summary>
    public static class ProjectSetup
    {
        const string CityScenePath = "Assets/Scenes/City.unity";
        const string BlackjackScenePath = "Assets/Scenes/Blackjack.unity";
        const string ArtRoot = "Assets/Resources";

        const string SettingsDir = "Assets/Settings";
        const string RendererPath = SettingsDir + "/URP Renderer.asset";
        const string PipelinePath = SettingsDir + "/URP Pipeline.asset";

        [MenuItem("Starflare/Rebuild Project Setup")]
        public static void Run()
        {
            ConfigurePlayer();
            ConfigureRenderPipeline();
            ReimportArt();
            BuildScene(CityScenePath, "City", typeof(City));
            BuildScene(BlackjackScenePath, "Blackjack Table", typeof(BlackjackTable));

            // The city is the build's first scene; the blackjack table stays for testing.
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(CityScenePath, true),
                new EditorBuildSettingsScene(BlackjackScenePath, true),
            };
            AssetDatabase.SaveAssets();
        }

        /// <summary>Art imported before <see cref="ArtImporter"/> existed picks up its settings.</summary>
        static void ReimportArt()
        {
            foreach (var folder in new[] { "Plates", "Poses", "Inserts" })
            {
                string path = $"{ArtRoot}/{folder}";
                if (AssetDatabase.IsValidFolder(path))
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            }
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
            // Portrait first, but every screen has a landscape layout too, so phones may turn.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
        }

        static void BuildScene(string path, string rootName, System.Type component)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = new GameObject("Main Camera", typeof(Camera));
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;

            new GameObject(rootName, component);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            EditorSceneManager.SaveScene(scene, path);
        }
    }
}
