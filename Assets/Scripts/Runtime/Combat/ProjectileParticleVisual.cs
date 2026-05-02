using UnityEngine;

namespace Animalis.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Projectile))]
    public sealed class ProjectileParticleVisual : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private SpriteRenderer coreRenderer;
        [SerializeField] private bool restartOnEnable = true;

        public void ApplyColor(Color tint)
        {
            ResolveReferences();

            if (coreRenderer != null)
            {
                coreRenderer.color = tint;
            }

            if (trail != null)
            {
                trail.startColor = tint;
                trail.endColor = tint;
            }

            if (particleSystems == null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem == null)
                {
                    continue;
                }

                ParticleSystem.MainModule main = particleSystem.main;
                main.startColor = tint;
            }
        }

        public void StopEmitting()
        {
            ResolveReferences();

            if (trail != null)
            {
                trail.emitting = false;
            }

            if (particleSystems == null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (!restartOnEnable)
            {
                return;
            }

            if (trail != null)
            {
                trail.Clear();
                trail.emitting = true;
            }

            RestartParticles();
        }

        private void OnDisable()
        {
            if (trail != null)
            {
                trail.Clear();
            }

            if (particleSystems == null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
        }

        private void RestartParticles()
        {
            if (particleSystems == null)
            {
                return;
            }

            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem == null)
                {
                    continue;
                }

                particleSystem.Clear(true);
                particleSystem.Play(true);
            }
        }

        private void ResolveReferences()
        {
            if (coreRenderer == null)
            {
                coreRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (trail == null)
            {
                trail = GetComponent<TrailRenderer>();
            }

            if (particleSystems == null || particleSystems.Length == 0)
            {
                particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            }
        }
    }
}
