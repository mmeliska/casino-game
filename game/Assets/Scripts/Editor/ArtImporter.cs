using UnityEditor;
using UnityEngine;

namespace Starflare.EditorTools
{
    /// <summary>
    /// Imports scene art (Resources/Plates, Poses and Inserts, and each art style's copy of them
    /// under Resources/Styles), the crowd faces (Resources/Faces), the scratcher tickets
    /// (Resources/Scratchers), the bowling pictures (Resources/Bowling) and the city map (Resources/Map)
    /// as UI sprites, and the bowling lane's textures (Resources/Bowling/lane_*) as repeating
    /// textures, so new images dropped in under their tag names work without touching the
    /// import settings.
    /// </summary>
    public sealed class ArtImporter : AssetPostprocessor
    {
        // Styles/ holds each other art style's Plates, Poses and Inserts (ArtStyles).
        static readonly string[] Folders = { "Assets/Resources/Plates/", "Assets/Resources/Poses/", "Assets/Resources/Inserts/", "Assets/Resources/Faces/", "Assets/Resources/Scratchers/", "Assets/Resources/Bowling/", "Assets/Resources/Styles/", MapFolder };
        const string MapFolder = "Assets/Resources/Map/";

        void OnPreprocessTexture()
        {
            if (IsLaneTexture(assetPath))
            {
                LaneTexture((TextureImporter)assetImporter);
                return;
            }
            bool isArt = false;
            foreach (var folder in Folders) isArt |= assetPath.StartsWith(folder);
            if (!isArt) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            // The map zooms from the whole city down to a block, so it keeps its full size and
            // mipmaps to stay clean zoomed out.
            bool map = assetPath.StartsWith(MapFolder);
            importer.mipmapEnabled = map;
            importer.maxTextureSize = map ? 4096 : 2048;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }

        /// <summary>
        /// The bowling lane's textures (Resources/Bowling/lane_*.png, and each style's copy) tile
        /// down the lane in perspective, so they repeat and keep their mipmaps instead of being sprites.
        /// </summary>
        static bool IsLaneTexture(string path) =>
            path.StartsWith("Assets/Resources/") && path.Contains("/Bowling/lane_");

        static void LaneTexture(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Trilinear;
            importer.anisoLevel = 4;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }
}
