using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace Magiclab.PackageManager.Editor
{
    public class MagiclabPackageManager : EditorWindow
    {
        private static List<MagiclabPackage> _magiclabPackages;

        private bool _staging;
        private bool _isDirty;
        private bool _readyForListRequest;
        private bool _isInitialized;
        private bool _isFetching;
        private bool _isPulledPackage;
        private Vector2 _scrollViewPos;
        private UnityWebRequest _fetchPackagesRequest;
        private ListRequest _listRequest;

        private GUIStyle _majorHeaderStyle;
        private GUIStyle _packageRowStyle;
        private GUIStyle _dependenciesHeaderStyle;
        private GUIStyle _dependenciesRowStyle;
        
        private static string _activityMessage;

        private const string MagiclabPackagesUrl = "https://rpi2ze66p5.execute-api.eu-central-1.amazonaws.com/Prod/api/v1/sdk/list";
        private const string XAPIKey = "afZe4FIAWc2tgdYuuBmETaSAYFw8qdyj1H1q5KCe";

        private void Awake()
        {
            _majorHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 25
            };
            _packageRowStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal
            };
            _dependenciesHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 16
            };
            _dependenciesRowStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                fixedHeight = 16
            };
        }
        
        public static void Init(bool stage)
        {
            var window = GetWindow<MagiclabPackageManager>(true);
            var minWindowSize = new Vector2(770f, 400f);
            window.minSize = minWindowSize;
            window._staging = stage;
            window.titleContent = new GUIContent("Magiclab Package Manager");
            window.Focus();
        }

        private void OnEnable()
        {
            PackageManifestManager.OnProgressCompleted += PackageManifestManager_OnProgressCompleted;
            Reload();
        }

        private void OnDisable()
        {
            PackageManifestManager.OnProgressCompleted -= PackageManifestManager_OnProgressCompleted;
        }

        private void Reload()
        {
            if (PackageManifestManager.PackageManagerAddRequest != null)
            {
                _activityMessage = "Installing package...";
            }
            else if (PackageManifestManager.PackageManagerRemoveRequest != null)
            {
                _activityMessage = "Removing package...";
            }
            else
            {
                _isInitialized = false;
                PullMagiclabPackages();
            }
        }

        private void PullMagiclabPackages()
        {
            _isFetching = true;
            _isPulledPackage = false;
            _activityMessage = "Fetching packages...";

            _fetchPackagesRequest = GetJsonWebRequest();
            _fetchPackagesRequest.SendWebRequest();
        }

        public UnityWebRequest GetJsonWebRequest()
        {
            var webRequest = new UnityWebRequest(MagiclabPackagesUrl);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", XAPIKey);
            webRequest.method = UnityWebRequest.kHttpVerbGET;

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 10;

            return webRequest;
        }

        private void CreateMagiclabPackages(MagiclabPackageConfigs magiclabPackageConfigs)
        {
            _activityMessage = "Creating package list...";
            _magiclabPackages = new List<MagiclabPackage>();
            Task.Run(async () =>
            {
                foreach (var packageInfo in magiclabPackageConfigs.magiclabPackageConfigs)
                {
                    var magiclabPackage = new MagiclabPackage(packageInfo.packageName, packageInfo.packageDisplayName, 
                        packageInfo.packageLink);
                    await magiclabPackage.Init();
                    
                    if (magiclabPackage.Hashes.Count <= 0) continue;
                    
                    magiclabPackage.SetMagiclabPackageDependencies(packageInfo.magiclabPackageDependencies);
                    _magiclabPackages.Add(magiclabPackage);
                }

                if (_magiclabPackages.Count <= 0)
                {
                    _activityMessage = "No available package.";
                }

                _readyForListRequest = true;
            });
        }

        private void ListRequestProgress()
        {
            if (_listRequest.IsCompleted)
            {
                if (_listRequest.Status == StatusCode.Success)
                {
                    foreach (var magiclabPackage in _magiclabPackages)
                    {
                        bool isInstalled = false;
                        foreach (var packageInfo in _listRequest.Result)
                        {
                            if (packageInfo.name != magiclabPackage.PackageName) continue;

                            isInstalled = true;
                            magiclabPackage.CheckInstalledPackage(packageInfo);
                            break;
                        }

                        if (!isInstalled)
                        {
                            magiclabPackage.CheckInstalledPackage(null);
                        }
                    }
                }
                else if (_listRequest.Status == StatusCode.InProgress)
                {
                    _activityMessage = "Searching installed packages...";
                }
                else
                {
                    Debug.LogError(_listRequest.Error.message);
                }

                EditorApplication.update -= ListRequestProgress;
                _isDirty = true;
            }
        }

        private void Update()
        {
            CheckFetchRequest();

            if (_readyForListRequest)
            {
                _readyForListRequest = false;
                
                _listRequest = Client.List();
                EditorApplication.update += ListRequestProgress;
            }
            
            if (_isDirty)
            {
                _isDirty = false;
                _isInitialized = true;

                Repaint();
            }
        }

        private void CheckFetchRequest()
        {
            if (!_isFetching || !_fetchPackagesRequest.isDone) return;
            
            _isFetching = false;
            if (_fetchPackagesRequest.result == UnityWebRequest.Result.ConnectionError || _fetchPackagesRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                _isPulledPackage = false;
                _activityMessage = "Error fetching packages. Details on the console.";
                Debug.LogError(_fetchPackagesRequest.error);
                _isInitialized = true;
            }
            else
            {
                _isPulledPackage = true;
                var packagesJson = _fetchPackagesRequest.downloadHandler.text;
                var magiclabPackageConfigs = JsonUtility.FromJson<MagiclabPackageConfigs>(packagesJson);
                CreateMagiclabPackages(magiclabPackageConfigs);
            }
        }

        private void OnGUI()
        {
            if (!_isFetching && !_isInitialized && !_isPulledPackage && !PackageManifestManager.IsBusy) return;
            
            GUILayout.Space(20);

            PackageHeaders();
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollViewPos))
            {
                _scrollViewPos = scrollViewScope.scrollPosition;
                if (!_isFetching && _isPulledPackage && _isInitialized && !PackageManifestManager.IsBusy && _magiclabPackages.Count > 0)
                {
                    foreach (var magiclabPackage in _magiclabPackages)
                    {
                        PackageRow(magiclabPackage);
                    }
                }
                else
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField(_activityMessage, EditorStyles.boldLabel);
                    }
                }
            }

            using (new EditorGUI.DisabledGroupScope(!_isInitialized || PackageManifestManager.IsBusy))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reload",GUILayout.Height(30)))
                {
                    Reload();
                }
                GUILayout.Space(20);
            }
        }

        private void PackageHeaders()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Packages", _majorHeaderStyle, GUILayout.Width(250f));
                EditorGUILayout.LabelField("Installed Version", _majorHeaderStyle, GUILayout.Width(150f));
                EditorGUILayout.LabelField("Version", _majorHeaderStyle, GUILayout.Width(120f));
                GUILayout.Button("Action", _majorHeaderStyle);
                GUILayout.Space(20);
            }
        }

        private void PackageRow(MagiclabPackage package)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(package.PackageDisplayName, _packageRowStyle, GUILayout.Width(250f));
                EditorGUILayout.LabelField(package.InstalledVersionLabel, _packageRowStyle, GUILayout.Width(150f));
                
                package.SelectedVersionIndex = EditorGUILayout.Popup(package.SelectedVersionIndex, package.VersionLabels, GUILayout.Width(120f));

                using (new EditorGUI.DisabledGroupScope(package.IsEmbedded || !package.IsInstalled))
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(100f)))
                    {
                        RemovePackage(package);
                    }
                }

                var isDisabledInstallButton = package.IsEmbedded || 
                                              package.IsInstalled && package.InstalledVersionIndex == package.SelectedVersionIndex;
                using (new EditorGUI.DisabledGroupScope(isDisabledInstallButton))
                {
                    var buttonName = "Install";
                    if (package.IsInstalled && package.InstalledVersionIndex != -1)
                    {
                        if (package.InstalledVersionIndex > package.SelectedVersionIndex)
                            buttonName = "Upgrade";
                        else if (package.InstalledVersionIndex < package.SelectedVersionIndex)
                            buttonName = "Downgrade";
                    }
                    if (GUILayout.Button(buttonName, GUILayout.Width(100f)))
                    {
                        InstallPackage(package);
                    }   
                }
                GUILayout.Space(20);
            }
            WriteDependencies(package);
        }

        private void WriteDependencies(MagiclabPackage package)
        {
            if (package.MagiclabPackageDependencies == null || package.MagiclabPackageDependencies.Length <= 0) return;
            
            DependenciesHeaders();
            foreach (var packageDependency in package.MagiclabPackageDependencies)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var packageName = packageDependency.packageName;
                    foreach (var magiclabPackage in _magiclabPackages)
                    {
                        if (magiclabPackage.PackageName != packageDependency.packageName) continue;
                        
                        packageName = magiclabPackage.PackageDisplayName;
                        break;
                    }
                    
                    GUILayout.Space(60);
                    EditorGUILayout.LabelField($"- {packageName}", _dependenciesRowStyle, GUILayout.Width(360));
                    EditorGUILayout.LabelField($"{packageDependency.version}", _dependenciesRowStyle, GUILayout.Width(120));
                }
            }
        }

        private void DependenciesHeaders()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(60);
                EditorGUILayout.LabelField("Dependencies:", _dependenciesHeaderStyle, GUILayout.Width(355));
                EditorGUILayout.LabelField("Minimum Version", _dependenciesHeaderStyle, GUILayout.Width(120));
            }
        }

        private void RemovePackage(MagiclabPackage package)
        {
            _activityMessage = $"Removing \"{package.PackageDisplayName}\" package...";
            PackageManifestManager.RemovePackage(package);
        }

        private void InstallPackage(MagiclabPackage package, Version version = null)
        {
            _activityMessage = $"Installing \"{package.PackageDisplayName}\" package...";
            PackageManifestManager.InstallPackage(package, version);
            
            CheckDependencies(package);
        }

        private void CheckDependencies(MagiclabPackage package)
        {
            if (package.MagiclabPackageDependencies == null) return;
            foreach (var dependencyInfo in package.MagiclabPackageDependencies)
            {
                if (!Version.TryParse(dependencyInfo.version, out var dependencyVersion))
                    continue;
        
                var dependencyPackage = new MagiclabPackage("", "", "");
                foreach (var magiclabPackage in _magiclabPackages)
                {
                    if (magiclabPackage.PackageName != dependencyInfo.packageName) continue;
                    
                    dependencyPackage = magiclabPackage;
                    break;
                }
                if (string.IsNullOrEmpty(dependencyPackage.PackageName)) continue;
        
                if (dependencyPackage.IsInstalled && dependencyPackage.InstalledVersion >= dependencyVersion) continue;
                
                var message = $"\"{package.PackageDisplayName}\" package require \"{dependencyPackage.PackageDisplayName}\" package with minimum \"{dependencyInfo.version}\" version.";
                EditorUtility.DisplayDialog("Install Dependencies", message, "OK");
                Debug.LogWarningFormat("\"{0}\" package require \"{1}\" package with minimum \"{2}\" version.",
                    package.PackageDisplayName, dependencyPackage.PackageDisplayName, dependencyInfo.version);
            }
        }

        private void PackageManifestManager_OnProgressCompleted()
        {
            Reload();
        }
    }
}