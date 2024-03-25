using DG.Tweening;
using UnityEngine;

namespace Novel.SO
{
    [CreateAssetMenu(fileName = nameof(CharacterConfig), menuName = "Project/SO/Character/Config", order = 0)]
    public sealed class CharacterConfig : ScriptableObject
    {
        [SerializeField, Range(0.1f, 5.0f)] private float _duration = 1.0f;
        [SerializeField] private Ease _ease = Ease.Linear;
        [SerializeField] private LayerMask _layer;

        public float Duration => _duration;
        public Ease Ease => _ease;
        public LayerMask Layer => _layer;
    }
}