using System;
using Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Novel.Camera
{
    public interface ICameraService
    {
        UnityEngine.Camera Camera { get; }
        
        void Initialize();
        void SetBlendData(CinemachineBlendDefinition.Style type, float time);
        void ResetFocusPriorityToDefault();
        void SetFocusPriority();
        void FocusOn(Transform target);
    }

    public sealed class CameraService : ICameraService
    {
        private const string MAIN = "Main";
        private const string FOCUS = "Focus";

        private UnityEngine.Camera _camera;
        private CinemachineStateDrivenCamera _stateDrivenCamera;
        private CinemachineVirtualCamera _main;
        private CinemachineVirtualCamera _focus;
        private Animator _stateDriveCameraAnimator;

        public UnityEngine.Camera Camera => _camera;

        void ICameraService.Initialize()
        {
            _stateDrivenCamera = Object.FindObjectOfType<CinemachineStateDrivenCamera>();
            _main = (CinemachineVirtualCamera)_stateDrivenCamera.ChildCameras[0];
            _focus = (CinemachineVirtualCamera)_stateDrivenCamera.ChildCameras[1];
            _stateDriveCameraAnimator = _stateDrivenCamera.m_AnimatedTarget;
            _camera = UnityEngine.Camera.main;
            ResetFocusPriorityToDefault();
        }

        public void SetBlendData(CinemachineBlendDefinition.Style type, float time)
        {
            _stateDrivenCamera.m_DefaultBlend = new CinemachineBlendDefinition(type, time);
        }

        public void ResetFocusPriorityToDefault()
        {
            _stateDriveCameraAnimator.Play(MAIN);
        }

        public void SetFocusPriority()
        {
            if (_focus == null) throw new NullReferenceException("Focus camera is null!");
        
            _stateDriveCameraAnimator.Play(FOCUS);
        }

        public void FocusOn(Transform target)
        {
            if (_focus == null) throw new NullReferenceException("Focus camera is null!");
            _focus.LookAt = target;
        }
    }
}