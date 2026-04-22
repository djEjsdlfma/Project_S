using System;
using System.IO;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEditor;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.Editor.TempFiles
{
    public class TempFolderToggler : EditorWindow
    {
        private static string _cachedPackageRoot;

        private static string PackageRoot
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedPackageRoot)) return _cachedPackageRoot;
                _cachedPackageRoot = ComputePackageRoot();
                return _cachedPackageRoot;
            }
        }

        private static string TargetFolder => $"{PackageRoot}/TempFiles";
        private static string TempCreateRoot => PackageRoot;
        private static string PackagePath => $"{PackageRoot}/Editor/TempFiles/TempTemplate.unitypackage";

        static TempFolderToggler()
        {
            EditorApplication.projectChanged += InvalidateCache;
        }

        private static void InvalidateCache() => _cachedPackageRoot = null;

        [MenuItem("Tools/ScriptFinder_Pro/Temp Folder Toggler")]
        private static void OpenWindow()
        {
            GetWindow<TempFolderToggler>("Temp Folder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Temp folder manager", EditorStyles.boldLabel);
            GUILayout.Space(6);

            GUILayout.Label($"Computed PackageRoot: {PackageRoot}");
            GUILayout.Label($"TargetFolder: {TargetFolder}");
            GUILayout.Label($"PackagePath (asset): {PackagePath}");
            GUILayout.Space(6);

            if (GUILayout.Button("Create Temp from package (overwrite if exists)"))
            {
                CreateFromPackageOverwrite();
            }

            if (GUILayout.Button("Delete Temp (if exists)"))
            {
                DeleteTempFolder();
            }
            
            GUILayout.Space(8);
            if (GUILayout.Button("Recompute paths / clear cache"))
            {
                InvalidateCache();
                DevLog.Log("Package path cache cleared. Recomputed PackageRoot: " + PackageRoot);
            }
            
            // GUILayout.Space(4);
            // if (GUILayout.Button("Backup Temp to package (overwrite)"))
            // {
            //     BackupTempToPackage();
            // }
            
        }

        private static void CreateFromPackageOverwrite()
        {
            string absPackagePath = GetAbsolutePath(PackagePath);
            if (string.IsNullOrEmpty(absPackagePath) || !File.Exists(absPackagePath))
            {
                DevLog.LogError($"Package not found (abs): {absPackagePath}");
                return;
            }

            if (AssetDatabase.IsValidFolder(TargetFolder))
            {
                bool ok = EditorUtility.DisplayDialog(
                    "Overwrite Temp Folder?",
                    $"Target folder already exists: {TargetFolder}\nThis operation will delete it and import the package. Continue?",
                    "OK", "Cancel");
                if (!ok) return;
            }

            string placeholderAssetPath = $"{TargetFolder}/README.txt";
            bool placeholderCreated = false;

            try
            {
                if (AssetDatabase.IsValidFolder(TargetFolder))
                {
                    bool deleted = AssetDatabase.DeleteAsset(TargetFolder);
                    if (!deleted)
                    {
                        try
                        {
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(TargetFolder));
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(TargetFolder) + ".meta");
                        }
                        catch (Exception ex)
                        {
                            DevLog.LogError("Failed to delete target via FileUtil: " + ex);
                        }
                    }
                }
                else
                {
                    EnsureFolderExists(TargetFolder);
                }

                string absPlaceholder = GetAbsolutePath(placeholderAssetPath);
                if (!string.IsNullOrEmpty(absPlaceholder) && !File.Exists(absPlaceholder))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(absPlaceholder));
                    File.WriteAllText(absPlaceholder, "Auto-generated placeholder for import.");
                    AssetDatabase.ImportAsset(placeholderAssetPath);
                    placeholderCreated = true;
                }

                EnsureFolderExists(TempCreateRoot);

                AssetDatabase.ImportPackage(absPackagePath, false);

                AssetDatabase.Refresh();

                if (AssetDatabase.IsValidFolder(TargetFolder))
                {
                    DevLog.Log("Created target from package: " + TargetFolder);
                }
                else
                {
                    DevLog.LogError("Import completed but target folder not found: " + TargetFolder);
                }
            }
            catch (Exception e)
            {
                DevLog.LogError("Failed to create from package: " + e);
            }
            finally
            {
                if (placeholderCreated)
                {
                    if (!AssetDatabase.DeleteAsset(placeholderAssetPath))
                    {
                        try
                        {
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(placeholderAssetPath));
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(placeholderAssetPath) + ".meta");
                        }
                        catch (Exception ex)
                        {
                            DevLog.LogError("Failed to remove placeholder: " + ex);
                        }
                    }
                    AssetDatabase.Refresh();
                    DevLog.Log("Placeholder removed: " + placeholderAssetPath);
                }
            }
        }

        private static void DeleteTempFolder()
        {
            if (!AssetDatabase.IsValidFolder(TargetFolder))
            {
                DevLog.Log("Delete skipped. Target does not exist: " + TargetFolder);
                return;
            }

            bool ok = EditorUtility.DisplayDialog(
                "Delete Temp Folder",
                $"Are you sure you want to delete: {TargetFolder}\nThis cannot be undone.",
                "Delete", "Cancel");
            if (!ok) return;

            try
            {
                bool deleted = AssetDatabase.DeleteAsset(TargetFolder);
                if (!deleted)
                {
                    string abs = GetAbsolutePath(TargetFolder);
                    FileUtil.DeleteFileOrDirectory(abs);
                    FileUtil.DeleteFileOrDirectory(abs + ".meta");
                }

                AssetDatabase.Refresh();
                DevLog.Log("Deleted: " + TargetFolder);
            }
            catch (Exception e)
            {
                DevLog.LogError("Failed to delete target: " + e);
            }
        }

        private static void BackupTempToPackage()
        {
            string placeholderAssetPath = $"{TargetFolder}/README.txt";
            bool placeholderCreated = false;

            try
            {
                if (!AssetDatabase.IsValidFolder(TargetFolder))
                {
                    EnsureFolderExists(TargetFolder);

                    string absPlaceholder = GetAbsolutePath(placeholderAssetPath);
                    if (!string.IsNullOrEmpty(absPlaceholder) && !File.Exists(absPlaceholder))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(absPlaceholder));
                        File.WriteAllText(absPlaceholder, "Auto-generated placeholder for backup.");
                        AssetDatabase.ImportAsset(placeholderAssetPath);
                        placeholderCreated = true;
                    }
                }

                string absPackagePath = GetAbsolutePath(PackagePath);
                string packageDir = Path.GetDirectoryName(absPackagePath);
                if (!string.IsNullOrEmpty(packageDir))
                {
                    Directory.CreateDirectory(packageDir);
                }

                AssetDatabase.ExportPackage(new[] { TargetFolder }, absPackagePath, ExportPackageOptions.Recurse);

                AssetDatabase.Refresh();

                if (File.Exists(absPackagePath))
                    DevLog.Log("Backup package created at (abs): " + absPackagePath);
                else
                    DevLog.LogError("Failed to create backup package: " + absPackagePath);
            }
            catch (Exception e)
            {
                DevLog.LogError("Failed to backup to package: " + e);
            }
            finally
            {
                if (placeholderCreated)
                {
                    if (!AssetDatabase.DeleteAsset(placeholderAssetPath))
                    {
                        try
                        {
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(placeholderAssetPath));
                            FileUtil.DeleteFileOrDirectory(GetAbsolutePath(placeholderAssetPath) + ".meta");
                        }
                        catch (Exception ex)
                        {
                            DevLog.LogError("Failed to remove placeholder file: " + ex);
                        }
                    }
                    AssetDatabase.Refresh();
                    DevLog.Log("Placeholder file removed: " + placeholderAssetPath);
                }
            }
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath)) return;
            folderPath = folderPath.Replace("\\", "/").TrimEnd('/');
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string[] parts = folderPath.Split('/');
            if (parts.Length == 0) return;

            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(cur, parts[i]);
                }
                cur = next;
            }
        }

        private static string ComputePackageRoot()
        {
            string scriptName = nameof(TempFolderToggler);
            string filter = $"{scriptName} t:MonoScript";
            string[] guids = AssetDatabase.FindAssets(filter);

            if (guids != null && guids.Length > 0)
            {
                foreach (var g in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(g).Replace("\\", "/");
                    var ms = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (ms == null) continue;

                    Type scriptType = ms.GetClass();
                    if (scriptType != null && scriptType == typeof(TempFolderToggler))
                    {
                        string dir = Path.GetDirectoryName(path).Replace("\\", "/");
                        string found = WalkUpForPackageRoot(dir);
                        if (!string.IsNullOrEmpty(found)) return found;
                    }
                }

                string fallbackPath = AssetDatabase.GUIDToAssetPath(guids[0]).Replace("\\", "/");
                string fallbackDir = Path.GetDirectoryName(fallbackPath).Replace("\\", "/");
                string found2 = WalkUpForPackageRoot(fallbackDir);
                if (!string.IsNullOrEmpty(found2)) return found2;
            }

            DevLog.LogWarning("TempFolderToggler script not found via AssetDatabase. Falling back to 'Assets/Assets/ScriptFinder_Pro'.");
            return "Assets/ScriptFinder_Pro";
        }

        private static string WalkUpForPackageRoot(string startDir)
        {
            string dir = startDir;
            while (!string.IsNullOrEmpty(dir))
            {
                string folderName = Path.GetFileName(dir);
                if (string.Equals(folderName, "ScriptFinder_Pro", StringComparison.OrdinalIgnoreCase))
                {
                    return dir.Replace("\\", "/");
                }

                if (string.Equals(folderName, "Assets", StringComparison.OrdinalIgnoreCase))
                {
                    return "Assets";
                }

                dir = Path.GetDirectoryName(dir)?.Replace("\\", "/");
            }
            return null;
        }

        private static string GetAbsolutePath(string assetsPath)
        {
            if (string.IsNullOrEmpty(assetsPath)) return null;
            assetsPath = assetsPath.Replace("\\", "/").Trim();
            if (assetsPath.Equals("Assets", StringComparison.OrdinalIgnoreCase))
            {
                return Application.dataPath.Replace("\\", "/");
            }
            if (assetsPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                string relative = assetsPath.Substring("Assets/".Length);
                return Path.Combine(Application.dataPath, relative).Replace("\\", "/");
            }
            return Path.GetFullPath(assetsPath).Replace("\\", "/");
        }
    }
}
