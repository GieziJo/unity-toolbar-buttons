using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BennyKok.ToolbarButtons
{
    public class DefaultToolbarButtons
    {
        private const string scenesFolder = "Scenes";

        public static T[] GetAtPath<T>(string path)
        {
            ArrayList al = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                string temp = fileName.Replace("\\", "/");
                int index = temp.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += temp.Substring(index);

                System.Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            T[] result = new T[al.Count];

            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }

        [ToolbarButton("d_winbtn_win_max", "Open Terminal")]
        public static void OpenTerminal()
        {
            var projectPath = Directory.GetParent(Application.dataPath).FullName;

            Process cmd = new Process();
#if UNITY_EDITOR_WIN
            cmd.StartInfo.FileName = "cmd.exe";
#endif
#if UNITY_EDITOR_OSX
            cmd.StartInfo.FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
#endif
            cmd.StartInfo.WorkingDirectory = projectPath;
            cmd.Start();
        }

        [ToolbarButton("Folder Icon", "Open Folder")]
        public static void OpenFolder()
        {
            var projectPath = Directory.GetParent(Application.dataPath).FullName;

            Process cmd = new Process();
#if UNITY_EDITOR_WIN
            cmd.StartInfo.FileName = "explorer.exe";
#endif
            // #if UNITY_EDITOR_OSX
            //             cmd.StartInfo.FileName = "/bin/bash";
            // #endif
            cmd.StartInfo.Arguments = projectPath;
            cmd.Start();
        }

        [ToolbarButton(iconName = "Package Manager", tooltip = "Package Manager")]
        public static void ShowPackageManager()
        {
            UnityEditor.PackageManager.UI.Window.Open("");
        }


        [ToolbarButton("Settings", "Show Settings")]
        public static void ShowSettings()
        {
            var a = new GenericMenu();
            a.AddItem(new GUIContent("Project"), false, () => EditorApplication.ExecuteMenuItem("Edit/Project Settings..."));
            a.AddItem(new GUIContent("Preferences"), false, () => EditorApplication.ExecuteMenuItem("Edit/Preferences..."));
            a.ShowAsContext();
        }

        [ToolbarButton("UnityEditor.SceneHierarchyWindow", "Show Scenes")]
        public static void ShowScenes()
        {
            var a = new GenericMenu();
            var ls = GetAtPath<UnityEngine.Object>(scenesFolder);
            foreach (var l in ls)
            {
                var p = AssetDatabase.GetAssetPath(l);
                var n = Path.GetFileName(p);
                if (n.EndsWith(".unity"))
                {
                    a.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(p)), false, () =>
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(p, OpenSceneMode.Single);
                            if (p == "bootstrap")
                            {
                                Selection.activeGameObject = GameObject.FindGameObjectWithTag("Player");
                                SceneView.FrameLastActiveSceneView();
                            }
                        }
                    });
                }
            }
            a.AddSeparator("");
            a.AddItem(new GUIContent("New Scene +"), false, () =>
            {
                EditorApplication.ExecuteMenuItem("File/New Scene");
            });
            a.ShowAsContext();
        }

        [ToolbarButton("UnityEditor.GameView", "Show Bootstrap Scene")]
        public static void ShowBootstrapScene()
        {
            var bootstrapPath = "Assets/" + scenesFolder + "/bootstrap.unity";
            if (!Application.isPlaying && File.Exists(bootstrapPath))
                EditorSceneManager.OpenScene(bootstrapPath, OpenSceneMode.Additive);
            Selection.activeGameObject = GameObject.FindGameObjectWithTag("Player");
            SceneView.FrameLastActiveSceneView();
        }

        public static void ShowQuickSearch()
        {
            EditorApplication.ExecuteMenuItem("Help/Quick Search");
        }

    }
}