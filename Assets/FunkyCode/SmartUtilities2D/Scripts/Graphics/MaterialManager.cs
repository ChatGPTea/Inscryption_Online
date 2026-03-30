using UnityEngine;

namespace FunkyCode.Utilities
{
    public class MaterialManager
    {
        private static SmartMaterial vertexLit;
        private static SmartMaterial additive;
        private static SmartMaterial alpha;
        private static SmartMaterial sprite;

        private static SmartMaterial GetVertexLit()
        {
            if (vertexLit == null || vertexLit.material == null)
            {
                //if (Slicer2DSettings.GetRenderingPipeline() == Slicer2DSettings.RenderingPipeline.Universal) {
                vertexLit = new SmartMaterial("Legacy Shaders/Transparent/VertexLit");
                //} else {
                //	vertexLit =  new SmartMaterial ("Sprites/Default");
                //}

                if (vertexLit != null) vertexLit.SetTexture(Resources.Load("Textures/LineTexture16") as Texture);
            }

            return vertexLit;
        }

        private static SmartMaterial GetAdditive()
        {
            if (additive == null || additive.material == null)
                additive = new SmartMaterial("Mobile/Particles/Additive");
            return additive;
        }

        private static SmartMaterial GetAlpha()
        {
            if (alpha == null || alpha.material == null) alpha = new SmartMaterial("Mobile/Particles/Alpha Blended");
            return alpha;
        }

        private static SmartMaterial GetSprite()
        {
            if (sprite == null || sprite.material == null) sprite = new SmartMaterial("Sprites/Default");
            return sprite;
        }

        public static SmartMaterial GetVertexLitCopy()
        {
            return new SmartMaterial(GetVertexLit());
        }

        public static SmartMaterial GetAdditiveCopy()
        {
            return new SmartMaterial(GetAdditive());
        }

        public static SmartMaterial GetAlphaCopy()
        {
            return new SmartMaterial(GetAlpha());
        }

        public static SmartMaterial GetSpriteCopy()
        {
            return new SmartMaterial(GetSprite());
        }
    }
}