using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using UnityEditor;
using Npu.EditorSupport;

namespace Npu
{

    public class PackageImporterWindow : EditorWindow
    {
        public string Path
        {
            get { return EditorPrefs.GetString("PackageImporterWindow-Path", "/Users/hiepnd/Downloads/SDKs"); }
            set { EditorPrefs.SetString("PackageImporterWindow-Path", value); }
        }

        public bool SilentImport
        {
            get { return EditorPrefs.GetBool("PackageImporterWindow-SilentImport"); }
            set { EditorPrefs.SetBool("PackageImporterWindow-SilentImport", value); }
        }

        string activePath = "";
        Dictionary<string, List<string>> packages = new Dictionary<string, List<string>>();

        [MenuItem("Npu/Tools/Package Importer")]
        public static void ShowMe()
        {
            var w = GetWindow(typeof(PackageImporterWindow)) as PackageImporterWindow;
            w.Show();
        }

        private void Awake()
        {
            packages = FindPackages();
            titleContent = new GUIContent("Package Importer");
        }

        private void OnGUI()
        {
            using (new HorizontalLayout())
            {
                Path = EditorGUILayout.TextField("Path", Path);

                if (GUILayout.Button("Load", GUILayout.Width(80)))
                {
                    packages = FindPackages();
                    //Debug.LogFormat("Packages: {0}", ToString(packages));
                }
            }

            SilentImport = EditorGUILayout.Toggle("Silent Import", SilentImport);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Found Packages (" + activePath + ")", EditorStyles.boldLabel);
            foreach (var i in packages)
            {
                var p = i.Key.Substring(activePath.Length);
                if (p.Length == 0) p = ".";

                EditorPrefs.SetBool(i.Key,
                    EditorGUILayout.Foldout(EditorPrefs.GetBool(i.Key, false), p + " (" + i.Value.Count + ")"));
                if (EditorPrefs.GetBool(i.Key))
                {
                    using (new Indent())
                    {
                        foreach (var k in i.Value)
                        {
                            using (new HorizontalLayout())
                            {
                                EditorGUILayout.LabelField(System.IO.Path.GetFileName(k));
                                if (GUILayout.Button("Import", GUILayout.Width(80)))
                                {
                                    Debug.Log("Importing " + k);
                                    AssetDatabase.ImportPackage(k, !SilentImport);
                                }
                            }
                        }
                    }
                }
            }
        }

        public Dictionary<string, List<string>> FindPackages()
        {
            activePath = Path;
            var packages = new Dictionary<string, List<string>>();
            FindPackages(Path, packages);
            return packages;
        }

        void FindPackages(string path, Dictionary<string, List<string>> packages)
        {
            var pks = FindPackages(path);
            if (pks.Count > 0) packages[path] = pks;
            foreach (var s in Directory.GetDirectories(path))
            {
                FindPackages(s, packages);
            }
        }

        public List<string> FindPackages(string path)
        {
            var packages = new List<string>();
            if (!Directory.Exists(path))
            {
                Debug.LogErrorFormat("Directory '{0}' does not exists", path);
                return packages;
            }

            packages.AddRange(
                Directory.GetFiles(path).Where(i => i.EndsWith(".unitypackage", StringComparison.Ordinal)));

            return packages;
        }
    }


}