using UnityEngine;

namespace Animalis.Core
{
    public readonly struct DamageData
    {
        public DamageData(GameObject source, float amount, Vector2 hitPoint, Vector2 direction)
        {
            Source = source;
            Amount = amount;
            HitPoint = hitPoint;
            Direction = direction;
        }

        public GameObject Source { get; }
        public float Amount { get; }
        public Vector2 HitPoint { get; }
        public Vector2 Direction { get; }
    }
}
