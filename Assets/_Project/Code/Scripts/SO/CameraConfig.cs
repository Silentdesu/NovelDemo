using Cinemachine;
using UnityEngine;

namespace Novel.SO
{
    [CreateAssetMenu(fileName = nameof(CameraConfig), menuName = "Project/SO/Camera/Config")]
    public sealed class CameraConfig : ScriptableObject
    {
        [SerializeField, Range(0.1f, 3.0f)] private float _focusTime = 2.0f;
        [SerializeField]
        private CinemachineBlendDefinition.Style _blendType = CinemachineBlendDefinition.Style.EaseInOut;

        public float FocusTime => _focusTime;

        public CinemachineBlendDefinition.Style BlendType => _blendType;
    }
}