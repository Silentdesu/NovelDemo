using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Novel
{
    [RequireComponent(typeof(SkeletonAnimation))]
    [DisallowMultipleComponent]
    public sealed class Character : MonoBehaviour
    {
        private const string IDLE_ANIMATION = "idle";
        private const int TALK_TRACK = 1;

        [SerializeField] private Collider _collider;
        [SerializeField] private SkeletonAnimation _skeletonAnimation;
        
        [Header("Animations")]

        [SerializeField, SpineAnimation] private string _idleAnimation;
        [SerializeField, SpineAnimation] private string _idleFocusAnimation;
        [SerializeField, SpineAnimation] private string _secretAnimation;
        [SerializeField, SpineAnimation] private string _talkAnimation;
        
        [SerializeField] private MeshRenderer _meshRenderer;
    
        private Transform _transform;
        
        private Material[] _materials;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }
#endif

        private void Awake()
        {
            _transform = transform;
            _spineAnimationState = _skeletonAnimation.AnimationState;
            _skeleton = _skeletonAnimation.Skeleton;
        }

        public void ToggleCollider(bool enable)
        {
            if (_collider == null) return;

            _collider.enabled = enable;
        }
        public void ToggleMeshVisibility(bool isVisible)
        {
            if (_meshRenderer.enabled == isVisible) return;
            
            _meshRenderer.enabled = isVisible;
        }

        public void MoveTo(Vector3 pos, float duration, Ease ease, Action onComplete = null)
        {
            _transform.DOMove(pos, duration).SetEase(ease).OnComplete(() => onComplete?.Invoke());
        }

        public void PlaySecretAnimation()
        {
            SetAnimation(_secretAnimation);
        }

        public void PlayTalkAnimation()
        {
            _spineAnimationState.SetAnimation(TALK_TRACK, _talkAnimation, true);
        }

        public void StopTalkAnimation()
        {
            _spineAnimationState.SetAnimation(TALK_TRACK, _talkAnimation, false);
        }

        public void PlayIdleFocusAnimation()
        {
            _spineAnimationState.SetAnimation(0, _idleFocusAnimation, true);
        }

        public void PlayIdleAnimation()
        {
            _spineAnimationState.SetAnimation(0, _idleAnimation, true);
        }

        private void SetAnimation(in string animName)
        {
            _spineAnimationState.SetAnimation(0, animName, false);
            _spineAnimationState.AddAnimation(0, IDLE_ANIMATION, true, 0.0f);
        }
    }
}