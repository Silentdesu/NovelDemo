using System;
using System.Collections;
// using Febucci.UI.Core;
using Novel.Dialogue;
using Novel.Infrastructure;
using Novel.Infrastructure.Inversion;
using Novel.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Novel.UI
{
    public interface IMainCanvas
    {
        // TypewriterCore TypewriterCore { get; }
        bool LineShown { get; set; }
        event Action<string> onLocaleButtonClick;
        event Action onPlayButtonClick;
        void SetCharacterName(string name);
        void SetDialogueText(string text, bool isCharacterMinds);
        void SetBackgroundText(string text);
        void ToggleTypeWriter(bool play);
        void ToggleMenuUI(bool enable);
    }

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public class MainCanvas : MonoBehaviour, IMainCanvas
    {
        [SerializeField] private TextMeshProUGUI _characterName;
        // [SerializeField] private TypewriterCore _typewriter;
        [SerializeField] private TextMeshProUGUI _continue;
        [SerializeField] private TextMeshProUGUI _background;
        [SerializeField] private RectTransform _dialogue;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _localeButton;

        private TextMeshProUGUI _dialogueText;
        private TextMeshProUGUI _buttonText;

        private bool _lineShown;

        // public TypewriterCore TypewriterCore => _typewriter;

        public bool LineShown
        {
            get => _lineShown;
            set
            {
                if (_continue != null)
                    _continue.gameObject.SetActive(value);
            }
        }

        public event Action<string> onLocaleButtonClick;
        public event Action onPlayButtonClick;

        private void Awake()
        {
            if (_localeButton)
            {
                _buttonText = _localeButton.GetComponentInChildren<TextMeshProUGUI>();
            }

            // if (_typewriter)
            // {
            //     _dialogueText = _typewriter.GetComponent<TextMeshProUGUI>();
            // }
        }

        private void OnEnable()
        {
            // if (_typewriter) _typewriter.onTextShowed.AddListener(() => LineShown = true);
            if (_localeButton) _localeButton.onClick.AddListener(OnLocaleButtonClick);
            if (_playButton) _playButton.onClick.AddListener(OnPlayButtonClick);
        }

        private void OnDisable()
        {
            if (_localeButton) _localeButton.onClick.RemoveListener(OnLocaleButtonClick);
            if (_playButton) _playButton.onClick.RemoveListener(OnPlayButtonClick);
        }

        private void Start()
        {
            ServiceLocator.For(this).Get<IGameManager>(out var gameManager);

            gameManager.onLanguageChangedEvent += OnLocaleChanged;
        }

        public void SetCharacterName(string name)
        {
            _characterName.SetText(name);
        }

        public void SetDialogueText(string text, bool isCharacterMinds = false)
        {
            _dialogueText.fontStyle = isCharacterMinds ? FontStyles.Italic : FontStyles.Normal;
            // _typewriter.ShowText(text);
        }

        public void SetBackgroundText(string text)
        {
            if (_background == null) return;
            
            _background.SetText(text);
        }

        public void ToggleTypeWriter(bool play)
        {
            if (Game.State == Game.EGameState.Menu) return;

            // if (play)
            //     _typewriter.StartShowingText();
            // else
            //     _typewriter.StopShowingText();
        }

        public void ToggleMenuUI(bool enable)
        {
            _playButton.gameObject.SetActive(enable);
            _localeButton.gameObject.SetActive(enable);
            _dialogue.gameObject.SetActive(!enable);
            ToggleTypeWriter(enable);
        }
        

        private void OnLocaleButtonClick()
        {
            onLocaleButtonClick?.Invoke(_buttonText.text);
        }

        private void OnPlayButtonClick()
        {
            onPlayButtonClick?.Invoke();
        }

        private void OnLocaleChanged(LocalizationService.ELocaleType language, DialogueSO.Parameters dialogueParams)
        {
            _buttonText.SetText(language.ToString());

            if (Game.State == Game.EGameState.Menu || dialogueParams.LocalizedString == default) return;

            StartCoroutine(GetLocalizedStringRoutine(dialogueParams));
        }

        private IEnumerator GetLocalizedStringRoutine(DialogueSO.Parameters dialogueParams)
        {
            var op = dialogueParams.LocalizedString.GetLocalizedStringAsync();
            yield return op;
            yield return null;
            
            if (op.Status == AsyncOperationStatus.Succeeded)
                SetDialogueText(op.Result, dialogueParams.IsCharacterMinds);
        }
    }
}