using System;

namespace Magiclab.PackageManager
{
    [Serializable]
    public struct MagiclabPackageConfigs
    {
        public MagiclabPackageConfig[] magiclabPackageConfigs;
    }
    
    [Serializable]
    public struct MagiclabPackageConfig
    {
        public string packageName;
        public string packageDisplayName;
        public string packageLink;
        public MagiclabPackageDependency[] magiclabPackageDependencies;
    }

    [Serializable]
    public struct MagiclabPackageDependency
    {
        public string packageName;
        public string version;
    }
}
