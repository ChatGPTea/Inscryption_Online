using System;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LightingMaterial
    {
        private Material material;
        private string path = "";
        private Texture texture;

        public static LightingMaterial Load(Material material)
        {
            var lightingMaterial = new LightingMaterial();

            //lightingMaterial.path = material.name;

            lightingMaterial.material = material;

            return lightingMaterial;
        }

        public static LightingMaterial Load(string path)
        {
            var lightingMaterial = new LightingMaterial();
            lightingMaterial.path = path;

            var shader = Shader.Find(path);
            if (!shader)
                Debug.LogError($"Smart Lighting: Shader Not Found '{path}'");
            else
                lightingMaterial.material = new Material(shader);

            return lightingMaterial;
        }

        public void SetTexture(string path)
        {
            texture = Resources.Load(path) as Texture;

            if (material) material.mainTexture = texture;
        }

        public void SetTexture(Texture setTexture)
        {
            texture = setTexture;

            if (material != null) material.mainTexture = texture;
        }

        public Material Get()
        {
            if (!material)
            {
                var shader = Shader.Find(path);
                if (shader)
                {
                    material = new Material(shader);
                    if (texture) material.mainTexture = texture;
                }
            }

            return material;
        }
    }
}