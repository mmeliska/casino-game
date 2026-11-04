using System.IO;
using Starflare.Game;
using TMPro;
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

        const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        const string TmpFontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

        const string SettingsDir = "Assets/Settings";
        const string RendererPath = SettingsDir + "/URP Renderer.asset";
        const string PipelinePath = SettingsDir + "/URP Pipeline.asset";

        [MenuItem("Starflare/Rebuild Project Setup")]
        public static void Run()
        {
            ConfigurePlayer();
            ConfigureRenderPipeline();
            SetUpTextMeshPro();
            ReimportArt();
            BuildScene(CityScenePath, "City", typeof(City));
            BuildScene(BlackjackScenePath, "Blackjack Table", typeof(BlackjackScene));

            // The city is the build's first scene; the blackjack table stays for testing.
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(CityScenePath, true),
                new EditorBuildSettingsScene(BlackjackScenePath, true),
            };
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Every label is TextMesh Pro (UiKit.MakeText), so the project needs TMP's essential
        /// resources: the settings asset and the default font. The import is the same one as
        /// Window > TextMeshPro > Import TMP Essential Resources. The font is then made dynamic,
        /// so glyphs outside its shipped atlas (the card suits, the ▼ prompt) render from the TTF.
        /// </summary>
        static void SetUpTextMeshPro()
        {
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath) == null)
            {
                string package = Path.Combine(Path.GetFullPath("Packages/com.unity.ugui"),
                    "Package Resources", "TMP Essential Resources.unitypackage");
                UnityEditor.AssetPackage.Package.Import(package, false);
                AssetDatabase.Refresh();
            }

            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TmpFontPath);
            if (font == null)
                throw new System.InvalidOperationException($"TextMesh Pro's default font wasn't imported at {TmpFontPath}.");
            if (font.atlasPopulationMode != AtlasPopulationMode.Dynamic)
            {
                font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
                font.isMultiAtlasTexturesEnabled = true;
                EditorUtility.SetDirty(font);
            }
        }

        /// <summary>Art imported before <see cref="ArtImporter"/> existed picks up its settings.</summary>
        static void ReimportArt()
        {
            foreach (var folder in new[] { "Plates", "Poses", "Inserts", "Map" })
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
            UseInputSystemOnly();
        }

        /// <summary>
        /// Input goes through the Input System package (uGUI's InputSystemUIInputModule and
        /// EnhancedTouch), so the legacy Input Manager is switched off. The setting has no API;
        /// it's the player settings' activeInputHandler (0 old, 1 both, 2 new) and takes effect
        /// after the editor restarts.
        /// </summary>
        static void UseInputSystemOnly()
        {
            var settings = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
            var handler = settings.FindProperty("activeInputHandler");
            if (handler == null || handler.intValue == 2) return;
            handler.intValue = 2;
            settings.ApplyModifiedPropertiesWithoutUndo();
            Debug.LogWarning("Active Input Handling is now Input System only. Restart the editor for it to take effect.");
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
