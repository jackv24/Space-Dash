using UnityEngine;
using System.Collections;
using UnityEditor;

public class ModelProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter importer = (ModelImporter)assetImporter;

        importer.materialImportMode = ModelImporterMaterialImportMode.None;
        importer.importNormals = ModelImporterNormals.Calculate;
        importer.normalSmoothingAngle = 0;
    }
}
