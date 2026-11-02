using UnityEditor;

namespace Starflare.EditorTools
{
    /// <summary>
    /// Imports scene art (Resources/Plates, Poses and Inserts) as UI sprites, so new images
    /// dropped in under their tag names work without touching the import settings.
    /// </summary>
    public sealed class ArtImporter : AssetPostprocessor
    {
        static readonly string[] Folders = { "Assets/Resources/Plates/", "Assets/Resources/Poses/", "Assets/Resources/Inserts/" };

        void OnPreprocessTexture()
        {
            bool isArt = false;
            foreach (var folder in Folders) isArt |= assetPath.StartsWith(folder);
            if (!isArt) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }
}
