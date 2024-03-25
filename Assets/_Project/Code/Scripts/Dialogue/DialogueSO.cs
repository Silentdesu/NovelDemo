using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Novel.Dialogue
{
    [CreateAssetMenu(fileName = nameof(DialogueSO), menuName = "Project/SO/Dialogue", order = -999)]
    public sealed class DialogueSO : ScriptableObject
    {
        [System.Serializable]
        public struct Parameters
        {
            public string Character;
            public LocalizedString LocalizedString;
            public bool IsCharacterMinds;
        }
    
        [SerializeField] private Parameters[] _data;
    
        public Parameters[] Data => _data;
    }
}