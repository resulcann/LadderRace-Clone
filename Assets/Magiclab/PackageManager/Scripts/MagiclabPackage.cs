using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager;

namespace Magiclab.PackageManager
{
    public class MagiclabPackage
    {
        public MagiclabPackage(string packageName, string packageDisplayName, string packageLink)
        {
            PackageName = packageName;
            PackageDisplayName = packageDisplayName;
            PackageLink = packageLink;
        }

        public string PackageName { get; }
        public string PackageDisplayName { get; }
        public string PackageLink { get; }
        public Dictionary<Version, string> Versions { get; private set; }
        public List<string> Hashes { get; private set; }
        public bool IsEmbedded { get; private set; }
        public bool IsInstalled { get; private set; }
        public int InstalledVersionIndex { get; private set; } = -1;
        public int SelectedVersionIndex { get; set; }
        public string InstalledVersionLabel { get; private set; }
        public string[] VersionLabels { get; private set; }
        public MagiclabPackageDependency[] MagiclabPackageDependencies { get; private set; }
        public Version InstalledVersion => Versions.ElementAt(InstalledVersionIndex).Key;

        public async Task Init()
        {
            var task = ExecuteProcessTerminal($"ls-remote --tags {PackageLink}");
            var output = await task;
            SetVersionsAndHashes(output);
        }

        public void CheckInstalledPackage(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                IsEmbedded = false;
                IsInstalled = false;
                
                UpdateInstalledVersionLabel();
            }
            else
            {
                IsEmbedded = packageInfo.source == PackageSource.Embedded;
                IsInstalled = !IsEmbedded;
                
                var installedHash = PackageManifestManager.GetHashInstalledVersion(this);
                SetInstalledVersion(installedHash);
            }
        }
        
        private async Task<string> ExecuteProcessTerminal(string argument)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
#if UNITY_EDITOR_OSX
                        FileName = "/bin/bash",
                        Arguments = " -c \"" + "git " + argument + " \"",
#elif UNITY_EDITOR_WIN
                        FileName = "git",
                        Arguments = argument,
#endif
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                    };
                    var myProcess = new Process
                    {
                        StartInfo = startInfo
                    };
                    myProcess.Start();
                    var output = myProcess.StandardOutput.ReadToEnd();
                    myProcess.WaitForExit();

                    return output;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning(e);
                    return null;
                }
            });
            return await task;
        }
        
        private void SetVersionsAndHashes(string commandOutput)
        {
            Versions = new Dictionary<Version, string>();
            Hashes = new List<string>();
            
            var lines = commandOutput.Split('\n');
            foreach (var line in lines)
            {
                if (!TryParseVersionAndHash(line, out var version, out var hash)) continue;
                
                Versions.Add(version, hash);
            }

            Versions = Versions.OrderByDescending(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
            
            VersionLabels = new string[Versions.Count];
            for (int i = 0; i < Versions.Count; i++)
            {
                VersionLabels[i] = Versions.ElementAt(i).Key.ToString();
            }
            
            foreach (var hash in Versions.Values)
            {
                Hashes.Add(hash);
            }
        }

        private bool TryParseVersionAndHash(string line, out Version version, out string hash)
        {
            version = new Version(0, 0, 0, 0);
            var info = line.Split(null);
            hash = info[0];
            
            if (string.IsNullOrEmpty(hash)) return false;
            
            var refInfo = info[1].Split('/');
            var versionInfo = refInfo[refInfo.Length - 1];
            
            if (!versionInfo.StartsWith("version") || !versionInfo.EndsWith("^{}")) return false;
                
            var versionSplitInfo = versionInfo.Split('_');
            var versionString = versionSplitInfo[1];
            versionString = versionString.Remove(versionString.Length - 3);
            if (!Version.TryParse(versionString, out version))
            {
                UnityEngine.Debug.LogErrorFormat("Invalid version tag for \"{0}\" package at \"{1}\" commit.", PackageDisplayName, hash);
                return false;
            }

            return true;
        }

        public void SetMagiclabPackageDependencies(MagiclabPackageDependency[] dependencies)
        {
            MagiclabPackageDependencies = dependencies;
        }

        public void SetInstalledVersion(string hash)
        {
            int installedVersionIndex = -1;
            for (int i = 0; i < Hashes.Count; i++)
            {
                if (Hashes[i] != hash) continue;
                
                installedVersionIndex = i;
                break;
            }
            
            InstalledVersionIndex = installedVersionIndex;
            UpdateInstalledVersionLabel();
        }

        private void UpdateInstalledVersionLabel()
        {
            if (IsEmbedded)
            {
                InstalledVersionLabel = "Embedded";
            }
            else 
            {
                if (IsInstalled)
                {
                    if (InstalledVersionIndex == -1)
                    {
                        InstalledVersionLabel = "UndefinedVersion";
                        UnityEngine.Debug.LogErrorFormat("Undefined version installed for \"{0}\" package.", PackageDisplayName);
                    }
                    else
                    {
                        InstalledVersionLabel = Versions.Keys.ToList()[InstalledVersionIndex].ToString();
                    }
                }
                else
                {
                    InstalledVersionLabel = "---";
                }
            }
        }

        public void RemovedPackage()
        {
            IsInstalled = false;
            InstalledVersionIndex = -1;
            UpdateInstalledVersionLabel();
        }
    }
}
