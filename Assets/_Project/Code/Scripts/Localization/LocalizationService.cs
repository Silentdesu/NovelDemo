using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Novel.Localization
{
    public interface ILocalizationService
    {
        bool TryChange(string language);
    }

    public sealed class LocalizationService : ILocalizationService
    {
        public enum ELocaleType
        {
            en = 0,
            ru
        }
        
        public bool TryChange(string language)
        {
            if (LocalizationSettings.SelectedLocale.LocaleName.Contains(language)) return false;
            
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (!locale.LocaleName.ToLower().Contains(language.ToLower())) continue;
                
                LocalizationSettings.SelectedLocale = locale;
                return true;
            }
            
            return false;
        }
    }
}