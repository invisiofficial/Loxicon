# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class LLamaSharpBuildPostprocessor 
{
    private static readonly string[] _libs = new string[] { "libllama.dll", "ggml.dll", "llava_shared.dll"};
    
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        foreach (string lib in _libs) CopyFile(lib, target, pathToBuiltProject);
    }
    
    public static void CopyFile(string lib, BuildTarget target, string pathToBuiltProject)
    {
        string pathToLibllama;
        pathToBuiltProject = Path.GetDirectoryName(pathToBuiltProject);
        if (target == BuildTarget.StandaloneWindows64)
        {
            pathToLibllama = Path.Join(
                Path.Join(
                    pathToBuiltProject, $"{PlayerSettings.productName}_Data", "Plugins"
                ), 
                "x86_64",
                lib
            );
        }
        else if (target == BuildTarget.StandaloneWindows)
        {
            pathToLibllama = Path.Join(
                Path.Join(
                    pathToBuiltProject, $"{PlayerSettings.productName}_Data", "Plugins"
                ), 
                "x86",
                lib
            );
        }
        else
        {
            Debug.LogError("Unsupported build target");
            return;
        }
        Debug.Log($"Copying {lib} from {pathToLibllama}");
        if (!File.Exists(pathToLibllama)) 
        {
            Debug.LogError($"{lib} not found in the built project");
            return;
        }
        // copy the libllama.dll to the project directory
        File.Copy(pathToLibllama, Path.Join(pathToBuiltProject, lib), true);
    }
}
#endif