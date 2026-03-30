using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightParticleSystem2D : MonoBehaviour
    {
        public enum Type
        {
            Particle
        }

        public static List<LightParticleSystem2D> List = new();

        public int lightLayer;

        public Color color = Color.white;

        public bool useParticleColor;

        public float scale = 1;

        public Texture customParticle;
        public ParticleSystem.Particle[] particleArray;

        private ParticleSystem particleSystem2D;
        private ParticleSystemRenderer particleSystemRenderer2D;

        public void OnEnable()
        {
            List.Add(this);

            LightingManager2D.Get();
        }

        public void OnDisable()
        {
            List.Remove(this);
        }

        public ParticleSystem GetParticleSystem()
        {
            if (particleSystem2D == null) particleSystem2D = GetComponent<ParticleSystem>();

            return particleSystem2D;
        }

        public ParticleSystemRenderer GetParticleSystemRenderer()
        {
            if (particleSystemRenderer2D == null) particleSystemRenderer2D = GetComponent<ParticleSystemRenderer>();

            return particleSystemRenderer2D;
        }
    }
}