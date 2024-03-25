#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Novel.Editor
{
    public sealed class Builder
    {
        

        [MenuItem("Tools/Novel/Builder/WebGL 🌐")]
        private static void BuildWebGL()
        {
            var buildReport = BuildPipeline.BuildPlayer(new()
            {
                target = BuildTarget.WebGL,
                locationPathName = "Build/WebGL",
                scenes = new[]
                {
                    "Assets/_Project/Level/_Scenes/Loader.unity",
                    "Assets/_Project/Level/_Scenes/Main.unity"
                }
            });
            
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build has succeeded! " +
                          $"Path [{buildReport.summary.outputPath}] | " +
                          $"Time [{buildReport.summary.totalTime}] | " +
                          $"Size [{buildReport.summary.totalSize}]");
            }
            else if (buildReport.summary.result == BuildResult.Failed)
            {
                Debug.LogError($"Build has failed! " +
                               $"Total errors [{buildReport.summary.totalErrors}] | " +
                               $"Total warnings [{buildReport.summary.totalWarnings}]");
            }
        }


        [MenuItem("Tools/Novel/Builder/Windows 💻")]
        private static void BuildWindows()
        {
            var buildReport = BuildPipeline.BuildPlayer(new()
            {
                target = BuildTarget.StandaloneWindows64,
                locationPathName = "Build/Windows/Novel.exe",
                scenes = new[]
                {
                    "Assets/_Project/Level/_Scenes/Loader.unity",
                    "Assets/_Project/Level/_Scenes/Main.unity"
                }
            });

            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build has succeeded! " +
                          $"Path [{buildReport.summary.outputPath}] | " +
                          $"Time [{buildReport.summary.totalTime}] | " +
                          $"Size [{buildReport.summary.totalSize}]");
            }
            else if (buildReport.summary.result == BuildResult.Failed)
            {
                Debug.LogError($"Build has failed! " +
                               $"Total errors [{buildReport.summary.totalErrors}] | " +
                               $"Total warnings [{buildReport.summary.totalWarnings}]");
            }
        }
    }
}
#endif