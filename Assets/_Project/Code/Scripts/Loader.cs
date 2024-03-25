using System.Collections;
using Novel.Dialogue;
using Novel.Infrastructure.Inversion;
using Novel.Input;
using Novel.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Novel
{
    [DisallowMultipleComponent]
    public sealed class Loader : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Global.Register<IInputService>(new InputService());
            ServiceLocator.Global.Register<IDialogueService>(new DialogueService());
            ServiceLocator.Global.Register<ILocalizationService>(new LocalizationService());
        }

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}