using Novel.Localization;
using UnityEngine;

namespace Novel.Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Project/SO/Game/Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private LocalizationService.ELocaleType _language;
        public LocalizationService.ELocaleType Language => _language;
    }
}