using System;
using System.Collections;
using DG.Tweening;
// using Febucci.UI.Core.Parsing;
using NaughtyAttributes;
using Novel.Camera;
using Novel.Configs;
using Novel.Dialogue;
using Novel.Infrastructure.Inversion;
using Novel.Input;
using Novel.Localization;
using Novel.SO;
using Novel.UI;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Novel.Utils.Constants;

namespace Novel.Infrastructure
{
    public static class Game
    {
        public enum EGameState
        {
            Menu = 0,
            Game
        }

        public static EGameState State = EGameState.Menu;
        public static LocalizationService.ELocaleType CurrentLanguage = LocalizationService.ELocaleType.en;
    }

    public interface IGameManager
    {
        event Action<LocalizationService.ELocaleType, DialogueSO.Parameters> onLanguageChangedEvent;
    }

    [DisallowMultipleComponent]
    public sealed class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField, Expandable] private GameConfig _gameConfig;
        [SerializeField, Expandable] private DialogueSO _dialogueSo;

        [Header("Character")]

        [SerializeField] private Character _character;

        [SerializeField, Expandable] private CharacterConfig _characterConfig;

        [Header("UI")]

        [SerializeField] private RectTransform[] _targets;

        [SerializeField] private RectTransform _targetOutsideCamera;

        [SerializeField] private MainCanvas _mainCanvas;

        [Header("Settings")]

        [SerializeField] private Transform _focusCameraTarget;

        [SerializeField, Expandable] private CameraConfig _cameraConfig;

        private IInputService _inputService;
        private ICameraService _cameraService;
        private IDialogueService _dialogueService;
        private ILocalizationService _localizationService;

        private DialogueSO.Parameters _dialogueParams;
        private RaycastHit[] _raycastHits;
        private int _targetIdx;
        private int _dialogueIdx = -1;
        private bool _waitForInput;

        private Vector3 _midTargetScreenPosition
        {
            get
            {
                var pos = UnityEngine.Camera.main.ScreenToWorldPoint(_targets[1].position);
                pos.z = _targets[1].position.z;
                return pos;
            }
        }
        
        private Vector3 _targetOutsideCameraPosition
        {
            get
            {
                var pos = UnityEngine.Camera.main.ScreenToWorldPoint(_targetOutsideCamera.position);
                pos.z = _targets[0].position.z;
                return pos;
            }
        }

        public event Action<LocalizationService.ELocaleType, DialogueSO.Parameters> onLanguageChangedEvent;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _mainCanvas = FindObjectOfType<MainCanvas>();
        }
#endif

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register<IGameManager>(this);
            ServiceLocator.ForSceneOf(this).Register<ICameraService>(new CameraService());
            ServiceLocator
                .For(this)
                .Get(out _inputService)
                .Get(out _cameraService)
                .Get(out _dialogueService)
                .Get(out _localizationService);

            _cameraService.Initialize();
            _dialogueService.SetDialogueData(_dialogueSo);

            _raycastHits = new RaycastHit[1];
        }

        private void Start()
        {
            if (_focusCameraTarget) _cameraService.FocusOn(_focusCameraTarget);

            OnLocaleClicked(_gameConfig.Language.ToString());
            UpdateCameraBlendData();
            SetGameState(Game.EGameState.Menu);
        }

        private void OnEnable()
        {
            if (_inputService != null)
            {
                _inputService.Enable();
                _inputService.onActionPerformedEvent += OnActionPerformed;
            }

            if (_mainCanvas /*&& _mainCanvas.TypewriterCore*/)
            {
                // _mainCanvas.TypewriterCore.onMessage.AddListener(OnMessage);
                _mainCanvas.onLocaleButtonClick += OnLocaleClicked;
                _mainCanvas.onPlayButtonClick += OnPlayClicked;
            }
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.Disable();
                _inputService.onActionPerformedEvent -= OnActionPerformed;
            }

            // if (_mainCanvas && _mainCanvas.TypewriterCore)
            //     _mainCanvas.TypewriterCore.onMessage.RemoveListener(OnMessage);
        }

        private void OnLocaleClicked(string language)
        {
            if (!_localizationService.TryChange(language)) return;
            if (!Enum.TryParse(typeof(LocalizationService.ELocaleType), language, true, out var type))
                return;

            Game.CurrentLanguage = type switch
            {
                LocalizationService.ELocaleType.en => LocalizationService.ELocaleType.ru,
                LocalizationService.ELocaleType.ru => LocalizationService.ELocaleType.en,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            onLanguageChangedEvent?.Invoke(Game.CurrentLanguage, _dialogueParams);
        }

        private void OnPlayClicked()
        {
            _character.ToggleMeshVisibility(isVisible: false);
            SetGameState(Game.EGameState.Game);
        }

        private void SetGameState(Game.EGameState state)
        {
            Game.State = state;

            if (state == Game.EGameState.Menu)
            {
                ChangeBackground("");
                SetGameData(_midTargetScreenPosition, true);
                _dialogueIdx = -1;
                _targetIdx = 0;
            }
            else
            {
                StopTalkAnimation();
                SetGameData(_targetOutsideCameraPosition, false);
                ContinueSequence();
            }
        }

        private void SetGameData(Vector3 charPos, bool inMenu)
        {
            _character.ToggleMeshVisibility(inMenu);
            _character.MoveTo(charPos, 0.0f, Ease.Linear);
            _mainCanvas.ToggleMenuUI(inMenu);
        }

        private void OnActionPerformed()
        {
            if (Game.State == Game.EGameState.Menu)
            {
                if (IsRaycastHit() > 0) PlaySecretAnimation();
                return;
            }

            // if (_mainCanvas.TypewriterCore.isShowingText) return;
            if (_waitForInput)
            {
                _waitForInput = false;
                _mainCanvas.ToggleTypeWriter(play: true);
                return;
            }

            ContinueSequence();
        }

        private void ContinueSequence()
        {
            _mainCanvas.LineShown = false;
            ++_dialogueIdx;

            if (_dialogueIdx < _dialogueService.Dialogue.Data.Length)
            {
                StartCoroutine(GetLocalizedStringRoutine());
            }
            else
            {
                _mainCanvas.SetCharacterName("");
                // _mainCanvas.TypewriterCore.StartDisappearingText();
                // _mainCanvas.TypewriterCore.onTextDisappeared.AddListener(() => { SetGameState(Game.EGameState.Menu); });
            }
        }

        private void MoveChar()
        {
            if (_targets == null || _targets.Length == 0)
                throw new NullReferenceException("Targets are null or empty!");

            if (_targetIdx == _targets.Length) return;

            var target = _targets[_targetIdx++];
            var targetWorldPos = UnityEngine.Camera.main.ScreenToWorldPoint(target.position);
            targetWorldPos.z = target.position.z;
            _character.ToggleMeshVisibility(isVisible: true);
            _character.MoveTo(targetWorldPos, _characterConfig.Duration, _characterConfig.Ease);
        }

        private void MoveCharToStartPosition()
        {
            _character.MoveTo(_targetOutsideCameraPosition, _characterConfig.Duration, Ease.Linear, 
                () => _character.ToggleMeshVisibility(false));
        }

        [Button("Focus camera on target", EButtonEnableMode.Playmode)]
        private void FocusCameraOnTarget()
        {
            if (_cameraService == null) throw new Exception("CameraService is null!");
            _cameraService.SetFocusPriority();
        }

        [Button("Reset to main camera", EButtonEnableMode.Playmode)]
        private void ResetToMainCamera()
        {
            if (_cameraService == null) throw new Exception("CameraService is null!");
            _cameraService.ResetFocusPriorityToDefault();
        }

        [Button("Update camera blend", EButtonEnableMode.Playmode)]
        private void UpdateCameraBlendData()
        {
            if (_cameraService == null) throw new Exception("CameraService is null!");
            _cameraService.SetBlendData(_cameraConfig.BlendType, _cameraConfig.FocusTime);
        }

        [Button("Play talk animation", EButtonEnableMode.Playmode)]
        private void PlayTalkAnimation()
        {
            _character.PlayTalkAnimation();
        }

        [Button("Stop talk animation", EButtonEnableMode.Playmode)]
        private void StopTalkAnimation()
        {
            _character.StopTalkAnimation();
        }

        [Button("Play secret animation", EButtonEnableMode.Playmode)]
        private void PlaySecretAnimation()
        {
            _character.PlaySecretAnimation();
        }

        [Button("Play idle animation", EButtonEnableMode.Playmode)]
        private void PlayIdleAnimation()
        {
            _character.PlayIdleAnimation();
        }

        [Button("Play idle focus animation", EButtonEnableMode.Playmode)]
        private void PlayIdleFocusAnimation()
        {
            _character.PlayIdleFocusAnimation();
        }

        private void OnMessage(string msg)
        {
            switch (msg)
            {
                case CHANGE_BACKGROUND:
                    ChangeBackground(msg);
                    break;
                case MOVE_CHARACTER:
                    MoveChar();
                    break;
                case MOVE_CHARACTER_OUTSIDE:
                    _targetIdx = 0;
                    MoveCharToStartPosition();
                    break;
                case CAMERA_FOCUS:
                    FocusCameraOnTarget();
                    break;
                case CAMERA_MAIN:
                    ResetToMainCamera();
                    break;
                case ANIMATION_TALK:
                    PlayTalkAnimation();
                    break;
                case ANIMATION_STOP_TALK:
                    StopTalkAnimation();
                    break;
                case ANIMATION_IDLE_FOCUS:
                    PlayIdleFocusAnimation();
                    break;
                case ANIMATION_SECRET:
                    PlaySecretAnimation();
                    break;
                case ACTION_CLICK:
                    _mainCanvas.ToggleTypeWriter(play: false);
                    _waitForInput = true;
                    break;
            }
        }

        private void ChangeBackground(string background)
        {
            var bgName = String.IsNullOrEmpty(background) ? "" : $"{background.Split('=')[0]}.jpg";
            _mainCanvas.SetBackgroundText(bgName);
        }

        private int IsRaycastHit()
        {
            var origin = _cameraService.Camera.ScreenToWorldPoint(_inputService.MousePosition);
            var ray = new Ray(origin, _cameraService.Camera.transform.forward);
            var hit = Physics.RaycastNonAlloc(ray, _raycastHits, float.MaxValue, _characterConfig.Layer);
            return hit;
        }

        private IEnumerator GetLocalizedStringRoutine()
        {
            _dialogueParams = _dialogueService.Dialogue.Data[_dialogueIdx];
            var op = _dialogueParams.LocalizedString.GetLocalizedStringAsync();
            yield return op;
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _mainCanvas.SetCharacterName(_dialogueParams.Character);
                _mainCanvas.SetDialogueText(op.Result,
                    _dialogueParams.IsCharacterMinds);
            }
        }

#if UNITY_EDITOR
        [Button("Move character", EButtonEnableMode.Playmode)]
        private void MoveCharDebug()
        {
            if (_targets == null || _targets.Length == 0)
                throw new NullReferenceException("Targets are null or empty!");

            var target = _targets[_targetIdx++ % _targets.Length];
            var targetWorldPos = UnityEngine.Camera.main.ScreenToWorldPoint(target.position);
            targetWorldPos.z = target.position.z;
            _character.MoveTo(targetWorldPos, _characterConfig.Duration, _characterConfig.Ease);
        }
#endif
    }
}