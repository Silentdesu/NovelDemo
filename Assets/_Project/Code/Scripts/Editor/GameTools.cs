#if UNITY_EDITOR
using Novel.SO;
using UnityEditor;

namespace Novel.Editor
{
    public sealed class GameTools
    {
        [MenuItem("Tools/Novel/Helpers/Ping Character Config")]
        private static void PingCharacterConfig()
        {
            var config = 
                AssetDatabase.LoadAssetAtPath<CharacterConfig>("Assets/_Project/Code/ScriptableObjects/Configs/CharacterConfig.asset");
            EditorGUIUtility.PingObject(config);
        }
        
        [MenuItem("Tools/Novel/Helpers/Ping Camera Config")]
        private static void PingCameraConfig()
        {
            var config = 
                AssetDatabase.LoadAssetAtPath<CameraConfig>("Assets/_Project/Code/ScriptableObjects/Configs/CameraConfig.asset");
            EditorGUIUtility.PingObject(config);
        }
    }
}
#endif