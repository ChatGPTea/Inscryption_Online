using UnityEngine;

namespace FunkyCode
{
    public class LightTexture
    {
        public int height;
        public RenderTexture renderTexture;
        public int setHeight;

        public int setWidth;

        public int width;

        public LightTexture(int width, int height, int depth, RenderTextureFormat format)
        {
            renderTexture = new RenderTexture(width, height, depth, format);

            this.width = width;
            this.height = height;
        }

        public LightTexture(int width, int height, int depth)
        {
            renderTexture = new RenderTexture(width, height, depth);

            this.width = width;
            this.height = height;
        }

        public void Create()
        {
            if (renderTexture != null) renderTexture.Create();
        }
    }
}