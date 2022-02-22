using UnityEditor;

namespace Magiclab.PackageManager.Editor
{
    public class MagiclabPackageManagerMenu
    {
        [MenuItem("Magiclab/Manage Packages...", false, 0)]
        public static void ManagePackages()
        {
            MagiclabPackageManager.Init(false);
        }
    }
}