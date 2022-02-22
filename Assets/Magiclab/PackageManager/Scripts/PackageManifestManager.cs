using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor;

namespace Magiclab.PackageManager
{
    public class PackageManifestManager
    {
        public static bool IsBusy { get; private set; }
        public static AddRequest PackageManagerAddRequest { get; private set; }
        public static RemoveRequest PackageManagerRemoveRequest { get; private set; }

        public static event Action OnProgressCompleted;
        
        private static string ManifestPath
        {
            get
            {
                var assetsPath = Application.dataPath;
                var projectFolderPath = assetsPath.Replace("/Assets", "");
                var manifestPath = Path.Combine(projectFolderPath + "/Packages/manifest.json");
                return manifestPath;
            }
        }

        private static string GetManifest()
        {
            var manifest = File.ReadAllText(ManifestPath);
            return manifest;
        }
        
        public static void RemovePackage(MagiclabPackage package)
        {
            if (PackageManagerRemoveRequest != null) return;
            
            PackageManagerRemoveRequest = Client.Remove(package.PackageName);
            IsBusy = true;
            EditorApplication.update += UnityPackageManagerRemoveProgress;
        }

        public static void InstallPackage(MagiclabPackage package, Version version)
        {
            if (PackageManagerAddRequest != null) return;
            
            var hash = version == null ? package.Hashes[package.SelectedVersionIndex] : package.Versions[version];
            PackageManagerAddRequest = Client.Add(package.PackageName + "@" + package.PackageLink + "#" + hash);
            IsBusy = true;
            EditorApplication.update += UnityPackageManagerAddProgress;
        }

        public static string GetHashInstalledVersion(MagiclabPackage package)
        {
            string pattern = package.PackageLink + @"#([A-Za-z0-9\-]+)";
            var manifest = GetManifest();
            var match = Regex.Match(manifest, pattern);
            var result = match.Value;
            result = result.Split('#')[1];
            return result;
        }

        private static void UnityPackageManagerAddProgress()
        {
            if (PackageManagerAddRequest.IsCompleted)
            {
                EditorApplication.update -= UnityPackageManagerAddProgress;
                
                OnProgressCompleted?.Invoke();
                IsBusy = false;
                
                if (PackageManagerAddRequest.Status == StatusCode.Success)
                    EditorUtility.DisplayDialog("Magiclab Package Manager", "Package installed successfully.", "OK");
                else if (PackageManagerAddRequest.Status == StatusCode.Failure)
                    EditorUtility.DisplayDialog("Magiclab Package Manager", "Failed to install the package!", "OK");

                PackageManagerAddRequest = null;
            }
        }
        
        private static void UnityPackageManagerRemoveProgress()
        {
            if (PackageManagerRemoveRequest.IsCompleted)
            {
                EditorApplication.update -= UnityPackageManagerRemoveProgress;
                
                OnProgressCompleted?.Invoke();
                IsBusy = false;
                
                if (PackageManagerRemoveRequest.Status == StatusCode.Success)
                    EditorUtility.DisplayDialog("Magiclab Package Manager", "Package removed successfully.", "OK");
                else if (PackageManagerRemoveRequest.Status == StatusCode.Failure)
                    EditorUtility.DisplayDialog("Magiclab Package Manager", "Failed to remove the package!", "OK");

                PackageManagerRemoveRequest = null;
            }
        }
    }
}
