using UnityEditor;

namespace Starflare.EditorTools
{
    /// <summary>
    /// Imports scene art (Resources/Plates, Poses and Inserts) and the city map (Resources/Map)
    /// as UI sprites, so new images dropped in under their tag names work without touching the
    /// import settings.
    /// </summary>
    public sealed class ArtImporter : AssetPostprocessor
    {
        static readonly string[] Folders = { "Assets/Resources/Plates/", "Assets/Resources/Poses/", "Assets/Resources/Inserts/", MapFolder };
        const string MapFolder = "Assets/Resources/Map/";

        void OnPreprocessTexture()
        {
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
    }
}
