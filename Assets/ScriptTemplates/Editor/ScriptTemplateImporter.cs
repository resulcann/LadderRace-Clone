// using System.IO;
// using UnityEditor;
//
// public class ScriptTemplateImporter : AssetPostprocessor
// {
//     private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
//         string[] movedFromAssetPaths)
//     {
//         foreach (var importedAsset in importedAssets)
//         {
//             if (importedAsset.StartsWith("Assets/ScriptTemplates/") && importedAsset.EndsWith(".txt"))
//             {
//                 DisplayRestartDialog();
//                 return;
//             }
//         }
//         
//         foreach (var movedAsset in movedAssets)
//         {
//             if (movedAsset.StartsWith("Assets/ScriptTemplates/") && movedAsset.EndsWith(".txt"))
//             {
//                 DisplayRestartDialog();
//                 return;
//             }
//         }
//     }
//
//     private static void DisplayRestartDialog()
//     {
//         var message = "A change has been made in ScriptTemplates folder. For the change to take effect, " +
//                       "you must restart the Editor. Would you like to reboot Editor now? " +
//                       "(You can also do this later.)";
//         if (EditorUtility.DisplayDialog("Script Template Importer", message, "Yes", "No"))
//         {
//             EditorApplication.OpenProject(Directory.GetCurrentDirectory());
//         }
//     }
// }
